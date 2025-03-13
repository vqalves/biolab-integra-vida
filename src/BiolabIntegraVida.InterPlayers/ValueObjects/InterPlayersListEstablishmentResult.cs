
using BiolabIntegraVida.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace BiolabIntegraVida.InterPlayers.ValueObjects
{
    public class InterPlayersListEstablishmentResult
    {
        public List<Establishment>? establishments { get; set; }
        public int? totalItemCount { get; set; }
        public int? page { get; set; }
        public int? pageCount { get; set; }
        public int? pageCurrent { get; set; }

        public class Establishment
        {
            public string? activityType { get; set; }
            public string? code { get; set; }
            public string? name { get; set; }
            public string? latitude { get; set; }
            public string? longitude { get; set; }
            public EstablishmentAddress? address { get; set; }
            public List<EstablishmentProduct>? product { get; set; }
            public EstablishmentContact? contact { get; set; }

            public string? GetFormattedName()
            {
                if (string.IsNullOrWhiteSpace(name))
                    return name;

                var regex = new Regex("-.*$", RegexOptions.Compiled);
                var formattedName = regex.Replace(name, string.Empty).Trim();
                return formattedName;
            }
        }

        public class EstablishmentAddress
        {
            public string? zip { get; set; }
            public string? addressType { get; set; }
            public string? address { get; set; }
            public string? addressNr { get; set; }
            public string? addressComplement { get; set; }
            public string? district { get; set; }
            public string? city { get; set; }
            public string? state { get; set; }

            private bool HasData(string? data)
            {
                if (string.IsNullOrWhiteSpace(data))
                    return false;

                if (string.Equals(data, "-", StringComparison.OrdinalIgnoreCase))
                    return false;

                return true;
            }

            public string FormatFullAddress()
            {
                var parts = new List<string>();

                if(HasData(address))
                {
                    if (HasData(addressType))
                    {
                        parts.Add(addressType!);
                        parts.Add(" ");
                    }

                    parts.Add(address!);

                    if (HasData(addressNr))
                    {
                        parts.Add(", ");
                        parts.Add(addressNr!);
                    }

                    if(HasData(addressComplement))
                        parts.Add(addressComplement!);
                }

                if (HasData(district))
                {
                    parts.Add(" - ");
                    parts.Add(district!);
                }

                if(HasData(city))
                {
                    parts.Add(", ");
                    parts.Add(city!);
                }

                if (HasData(state))
                {
                    parts.Add(" - ");
                    parts.Add(state!);
                }

                return string.Join(string.Empty, parts);
            }
        }

        public class EstablishmentProduct
        {
            public string? programId { get; set; }
        }

        public class EstablishmentContact
        {
            public string? dddPhone { get; set; }
            public string? phone { get; set; }
            public string? email { get; set; }

            public string? FormatFullPhone()
            {
                try
                {
                    var parts = new List<string>();

                    if (!string.IsNullOrWhiteSpace(dddPhone))
                    {
                        var ddd = TelefoneDDD.Parse(dddPhone);
                        parts.Add($"({ddd.Valor})");
                    }

                    if (!string.IsNullOrWhiteSpace(phone))
                    {
                        var phoneNumber = Telefone.Parse(phone);
                        parts.Add(phoneNumber.ComMascara);
                    }

                    return string.Join(" ", parts);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}