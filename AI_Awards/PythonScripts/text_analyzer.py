# This Python file uses the following encoding: utf-8

import argparse
import re
import sys
import io

from transformers import MBartForConditionalGeneration, MBartTokenizer

sys.stdin = io.TextIOWrapper(sys.stdin.buffer, encoding='utf-8')
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

def split_text_to_paragraphs(text):
    paragraphs = re.split(r"\n\s*", text)
    return paragraphs


def split_text_to_sentences(text):
    text = text.replace("тыс.", "тыс")
    text = text.replace(r"\d+\.\d+", r"\d*\,\d*")
    sentences = text.split(".")
    return sentences


def group_paragraphs(paragraphs, block_size):
    blocks = []
    current_block = ""

    for paragraph in paragraphs:
        if len(current_block) + len(paragraph) <= block_size:
            current_block += paragraph + " "
        else:
            blocks.append(current_block.strip())
            current_block = paragraph + " "

    if current_block:
        blocks.append(current_block.strip())

    return blocks


def get_summary(model, tokenizer, text_blocks):
    sum_text = ""
    for text_block in text_blocks:
        input_ids = tokenizer(
            [text_block],
            max_length=600,
            padding="max_length",
            truncation=True,
            return_tensors="pt",
        )["input_ids"]
        min_length = int(len(text_block) / 50)
        output_ids = model.generate(
            input_ids=input_ids,
            no_repeat_ngram_size=3,
            max_length=1300,
            min_length=min_length,
        )[0]

        out = tokenizer.decode(output_ids, skip_special_tokens=True)
        sum_text += "\n" + out
    return sum_text


def our_summary(text):
    sentences = split_text_to_sentences(text)
    text_blocks = group_paragraphs(sentences, block_size=6000)

    model_name = "IlyaGusev/mbart_ru_sum_gazeta"
    tokenizer_sum = MBartTokenizer.from_pretrained(model_name)
    model_sum = MBartForConditionalGeneration.from_pretrained(model_name)
    sum_text = get_summary(model_sum, tokenizer_sum, text_blocks)
    return sum_text


if __name__ == "__main__":
    my_parser = argparse.ArgumentParser()
    my_parser.add_argument("--text", type=str, help="text to summarize")
    args = my_parser.parse_args()
    sum_text = our_summary(args.text)

    print(f"{sum_text}")
