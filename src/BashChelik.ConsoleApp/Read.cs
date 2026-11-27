using System;
using System.Linq;

namespace BashChelik.ConsoleApp;

/// <summary>Reads the document in a reader through the public API and prints it on screen; nothing is written to disk.</summary>
internal static class Read
{
    public static int Run(string readerName)
    {
        var document = DocumentReader.ReadAll(readerName);
        var data = document.DocumentData;
        var person = document.PersonalData;
        var address = document.Address;

        Console.WriteLine($"Kind: {document.Kind}");
        Console.WriteLine($"Valid today: {document.IsValidOn(DateTime.Today)}");

        Section("Document data");
        Field("Number", data.Number);
        Field("Type code", data.TypeCode);
        Field("Serial number", data.SerialNumber);
        Field("Chip serial number", data.ChipSerialNumber);
        Field("Issue date", Date(data.IssueDate));
        Field("Expiry date", Date(data.ExpiryDate));
        Field("Issuing authority", data.IssuingAuthority);
        Field("Document name", data.DocumentName);

        Section("Personal data");
        Field("Personal number", person.PersonalNumber?.Value);
        Field("Checksum valid", person.PersonalNumber?.HasValidChecksum.ToString());
        Field("Surname", person.Surname);
        Field("Given name", person.GivenName);
        Field("Parent's given name", person.ParentGivenName);
        Field("Sex", person.Sex);
        Field("Date of birth", person.BirthDate is { } birth ? $"{birth.Day?.ToString("00") ?? "??"}.{birth.Month?.ToString("00") ?? "??"}.{birth.Year}" : null);
        Field("Place of birth", person.PlaceOfBirth);
        Field("Community of birth", person.CommunityOfBirth);
        Field("State of birth", person.StateOfBirth);
        Field("State of birth code", person.StateOfBirthCode);
        if (person.ForeignerData is { } foreigner)
        {
            Field("Nationality", foreigner.Nationality);
            Field("Status", foreigner.Status);
            Field("Purpose of stay", foreigner.PurposeOfStay);
            Field("Note", foreigner.Note);
        }

        Section("Address");
        Field("State", address.State);
        Field("Community", address.Community);
        Field("Place", address.Place);
        Field("Street", address.Street);
        Field("House number", address.HouseNumber);
        Field("House letter", address.HouseLetter);
        Field("Entrance", address.Entrance);
        Field("Floor", address.Floor);
        Field("Apartment number", address.ApartmentNumber);
        Field("Address date", Date(address.AddressDate));
        Field("Label", address.Label);

        Section("Portrait");
        Field("JPEG bytes", document.Portrait.ToJpeg().Length.ToString());

        Section("Holder certificates");
        foreach (var certificate in document.HolderCertificates)
            Console.WriteLine($"  {certificate.Purpose}: {certificate.Certificate.Subject}, valid {certificate.Certificate.NotBefore:dd.MM.yyyy} to {certificate.Certificate.NotAfter:dd.MM.yyyy}");
        if (!document.HolderCertificates.Any())
            Console.WriteLine("  none");

        return 0;
    }

    private static void Section(string name) => Console.WriteLine($"\n{name}:");

    private static void Field(string name, string? value) => Console.WriteLine($"  {name,-22} {value ?? "(none)"}");

    private static string? Date(DateTime? date) => date?.ToString("dd.MM.yyyy");
}
