# Financial-Parser-v1.00-


Financial Data Extractor – Version 1.0

This project was my first attempt at parsing and extracting financial data from company reports. The goal was to capture key metrics such as assets, liabilities, cash flows, and other figures to analyze long-term spending and identify trends.

Key Points:

First-time Experience: This was my first large-scale project using PDFs, regular expressions, and dictionaries.

Learning Opportunity: I gained hands-on experience with string manipulation, dictionaries, actions, and designing extraction logic for complex documents.

Challenges:

PDFs are coordinate-based, not sequential data. Text extraction can reorder, jump, or overlap unexpectedly.

My initial assumption that PDF text would be sequential caused the program to fail in some cases, even after normalization attempts.

Outcome: The extractor works partially but does not fully handle all PDF structures. Despite this, the project was invaluable for understanding data parsing, creative problem-solving, and program structure.

Technical Notes:

The code uses dictionaries, loops, and regular expressions for data extraction.

Copilot assisted with parts of the code, but the core logic and structure are my own.

The program is written in C#, focusing on manual control over extracted data rather than relying entirely on libraries.

This version serves as a foundation for future improvements, including handling OCR/OMR PDFs and building a full data pipeline for quantitative financial analysis.

