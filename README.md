# BashChelik

Reads Serbian electronic identity documents from their chip through any PC/SC card reader: the document data, the holder's personal data and address, the portrait and the holder's certificates.

- Documents: identity card (lična karta), foreigner identity card (lična karta za strance), residence permit (boravišna dozvola). Identity cards issued before August 2014 have all expired and are not supported.
- Platforms: .NET 11, .NET 10 and .NET Framework 4.7.2 or newer (through .NET Standard 2.0; tested on 4.8). Reading a card has been tried on Windows. On Linux (with pcsc-lite) and macOS the library builds and its tests pass, but reading a real card has not been tried yet.
- No Ministry of Interior DLL: the library talks to the chip itself.

## Install

```shell
dotnet add package BashChelik
```

## Read a document

```csharp
using BashChelik;

var readerName = DocumentReader.ListReaders()[0];
IdentityDocument document = DocumentReader.ReadAll(readerName);

Console.WriteLine($"{document.PersonalData.GivenName} {document.PersonalData.Surname}");
Console.WriteLine($"Personal number valid: {document.PersonalData.PersonalNumber?.HasValidChecksum}");
Console.WriteLine($"Document valid today: {document.IsValidOn(DateTime.Today)}");
File.WriteAllBytes("portrait.jpg", document.Portrait.ToJpeg());
```

To read only some parts, open the document and read what you need:

```csharp
using var reader = DocumentReader.Open(readerName);
Address address = reader.ReadAddress();
IReadOnlyList<HolderCertificate> certificates = reader.ReadCertificates();
```

A document that cannot be read throws `DocumentReadException`, whose `Reason` says why: no reader, no card, card removed, unsupported document or a chip error.

Dates are `DateTime?` with no time, and `null` when the document does not carry them. The date of birth is a `BirthDate`, which keeps the year when the day or month is unknown. The portrait is the JPEG stored on the chip; decode it with the image library you already use.

## Version 3

Versions 1 and 2 wrapped the Ministry's CelikAPI DLL and were never published as a package. Version 3 is a new library with a new API.

## Acknowledgments

Special thanks to my friend [Buda](https://twitter.com/buda81) for the initial project idea. [Saobracajna.NET](https://github.com/clearpath/Saobracajna.NET) was the inspiration for the first version's structure.
