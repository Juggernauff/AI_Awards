import sys
import os
import io
from concurrent.futures import ThreadPoolExecutor
from multiprocessing import cpu_count
import time
import numpy as np
import torch
import warnings
import logging


from transformers import AutoProcessor, AutoModelForSpeechSeq2Seq
from utils import convert_mp3_to_wav_with_new_sample_rate, load_audio_wav

warnings.filterwarnings("ignore")
logging.getLogger("transformers").setLevel(logging.ERROR) 

sys.stdin = io.TextIOWrapper(sys.stdin.buffer, encoding='utf-8')
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

# Load model and processor globally to avoid reloading in each function call
processor = AutoProcessor.from_pretrained("openai/whisper-large-v3")
model = AutoModelForSpeechSeq2Seq.from_pretrained("openai/whisper-large-v3")

def split_audio(audio, samplerate, chunk_duration=60):
    chunk_size = chunk_duration * samplerate
    return [audio[i:i + chunk_size] for i in range(0, len(audio), chunk_size)]

def transcribe_chunk(args):
    chunk, samplerate = args
    # Ensure the audio is in float32 format
    chunk = chunk.astype(np.float32)
    input_features = processor(chunk, sampling_rate=samplerate, return_tensors="pt").input_features

    # Generate transcription
    forced_decoder_ids = processor.get_decoder_prompt_ids(language="ru", task="transcribe")
    with torch.no_grad():
        predicted_ids = model.generate(input_features, forced_decoder_ids=forced_decoder_ids)

    # Decode the transcription
    transcription = processor.batch_decode(predicted_ids, skip_special_tokens=True)[0]
    return transcription

def transcribe_chunks_in_parallel(chunks, samplerate, max_workers=cpu_count()):
    transcriptions = []
    with ThreadPoolExecutor(max_workers=max_workers) as executor:
        futures = [executor.submit(transcribe_chunk, (chunk, samplerate)) for chunk in chunks]
        for future in futures:
            transcriptions.append(future.result())
    return " ".join(transcriptions)

def main(mp3_filename):
    # Здесь можно добавить доп папку к пути, например 
    if not os.path.exists(f"{mp3_filename}"):
        print(f"Error: File {mp3_filename} does not exist.")
        sys.exit(1)
        
    wav_filename = f"{mp3_filename}.wav"
    new_sample_rate = 16000  # Desired sample rate

    # Convert MP3 to WAV with the new sample rate
    convert_mp3_to_wav_with_new_sample_rate(mp3_filename, wav_filename, new_sample_rate)

    # Load and split the WAV audio
    audio, samplerate = load_audio_wav(wav_filename)
    chunks = split_audio(audio, samplerate)

    # Measure the duration of the transcription process
    start_time = time.time()
    transcription = transcribe_chunks_in_parallel(chunks, samplerate)
    end_time = time.time()

    print(transcription)

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python3 run.py <input_mp3_file>")
        sys.exit(1)
    
    mp3_filename = sys.argv[1]
    main(mp3_filename)