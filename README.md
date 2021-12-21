## PL/0 Interpreter

This repo is an experimental venture into interpreter building that revolves
around an extended version of the [PL/0 language](https://en.wikipedia.org/wiki/PL/0).
It is hand written for learning purposes.

```
usage: interpreter.exe [options] filename
       interpreter.exe [options]

Options

-d, --debug          Enables debug mode.
-p, --prompt         Enters in prompt mode.
-h, --help           Print this help message.
-v, --version        Print the version info.
```

### Features
 - Case-insensitive keywords
 - Integer, float, and string data types
 - Dynamic typing
 - C style typecasting
 - Integer literals can be hexadecimal, binary, or octal numbers ("0x", "0b", and "0" prefixes)
 - WRITE keyword or exclamation mark for print statements
 - READ keyword or question mark for input statements
 - Single line comments with hash character.

Coding conventions used are listed [here](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
