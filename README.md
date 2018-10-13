## PdfDocumentParser

### Prehistory
Once I needed to parse bunches of invoices issued by different companies and provided in PDF files. Invoices had predictable layouts so having a parsing template programmed for every layout they could be parsed very well. In the next step a GUI template editor was developed to allow creating and debugging templates in an easy and funny manner just with a mouse. Later, I decided to split the entire application onto a versatile library (PdfDocumentParser) that provides a set of pdf parsing routines and a custom desktop application (InvoiceParser) that uses this library. To keep PdfDocumentParser as flexible as possible, some important but restrictive features like keeping templates were left in the custom application. The main idea was that anybody who would use PdfDocumentParser, should not look much into PdfDocumentParser itself, but learn its usage from InvoiceParser instead. 

### Overview
PdfDocumentParser is a parsing engine designed to extract text/images from PDF documents conforming to a predefined graphic layout - such as invoices and the like. The main approach of parsing is based on finding certain text or image fragments in page and then extracting text/images located relatively to those fragments.

Within this scope PdfDocumentParser is capable to use the following approaches:
- PDF entity processing (the main way to operate with text in PdfDocumentParser);
- OCR (intended for scanned documents);
- image comparison (used either in native and scanned PDF files);

PdfDocumentParser was developed as a set of parsing tools that can be incorporated into a custom application hopefully without need of change. It provides:
- Template Editor where a PDF file can be open and a parsing template created or debugged;
- parsing engine API that can be used by a custom application: the engine accepts a PDF file and returns parsed data back to the calling code;

### Assumptions
- A PDF file can consist of more than one document (e.g. multiple invoices).
- A document (e.g. invoice) can consist of more than one page.

### InvoiceParser
[Invoice Parser](https://github.com/sergeystoyan/PdfDocumentParser/tree/lib%2Bcustomization/InvoiceParser) is a custom desktop application based on the leverages provided by [PdfDocumentParser](https://github.com/sergeystoyan/PdfDocumentParser) and developed for certain needs. Most likely it will not fit your needs from scratch but can be used as a sample.

### Support
Contact me through github if you want me to upgrade PdfDocumentParser or develop a custom application based on it.

[More details...](https://sergeystoyan.github.io/PdfDocumentParser/)