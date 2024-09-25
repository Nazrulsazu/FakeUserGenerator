using Bogus;
using System;
using FakeUserGenerator.Models;

namespace FakeUserGenerator.Services
{
    public class FakeUserService
    {
        public List<FakeUser> GenerateUserData(string region, int errorCount, Random random, int count, int startIndex)
        {
            // Instead of reusing a single faker object, we create separate Faker instances for each field
            // This ensures the locale is applied consistently

            List<FakeUser> users = new List<FakeUser>();

            for (int i = 0; i < count; i++)
            {
                var user = new FakeUser
                {
                    Index = startIndex + i,
                    RandomId = Guid.NewGuid().ToString(),
                    Name = GenerateName(region, random),
                    Address = GenerateAddress(region, random),
                    Phone = GeneratePhone(region, random)
                };

                // Use the same Random object to apply errors
                ApplyErrors(user, errorCount, random);
                users.Add(user);
            }

            return users;
        }

        private string GenerateName(string region, Random random)
        {
            var faker = new Faker(GetFakerLocale(region))
            {
                Random = new Bogus.Randomizer(random.Next())
            };
            return faker.Name.FullName();
        }

        private string GenerateAddress(string region, Random random)
        {
            var faker = new Faker(GetFakerLocale(region))
            {
                Random = new Bogus.Randomizer(random.Next())
            };
            return faker.Address.FullAddress();
        }

        private string GeneratePhone(string region, Random random)
        {
            var faker = new Faker(GetFakerLocale(region))
            {
                Random = new Bogus.Randomizer(random.Next())
            };
            return faker.Phone.PhoneNumber();
        }

        private string GetFakerLocale(string region)
        {
            return region switch
            {
                "Poland" => "pl",
                "USA" => "en",
                "Georgia" => "ge",
                _ => "en"  // Default to English if the region is not matched
            };
        }

        private void ApplyErrors(FakeUser user, int errorCount, Random random)
        {
            if (errorCount <= 0) return;

            for (int i = 0; i < errorCount; i++)
            {
                ApplyRandomError(user, random);
            }
        }

        private void ApplyRandomError(FakeUser user, Random random)
        {
            var fields = new List<string> { user.Name, user.Address, user.Phone };
            var fieldToError = random.Next(fields.Count);
            var selectedField = fields[fieldToError];

            var errorType = random.Next(3);  // 0: delete, 1: add, 2: swap
            switch (errorType)
            {
                case 0:
                    selectedField = DeleteCharacterAtRandom(selectedField, random);
                    break;
                case 1:
                    selectedField = AddCharacterAtRandom(selectedField, random);
                    break;
                case 2:
                    selectedField = SwapCharactersAtRandom(selectedField, random);
                    break;
            }

            // Update the corresponding field
            if (fieldToError == 0) user.Name = selectedField;
            else if (fieldToError == 1) user.Address = selectedField;
            else if (fieldToError == 2) user.Phone = selectedField;
        }

        private string DeleteCharacterAtRandom(string input, Random random)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var index = random.Next(input.Length);
            return input.Remove(index, 1);
        }

        private string AddCharacterAtRandom(string input, Random random)
        {
            var index = random.Next(input.Length);
            var charToAdd = (char)random.Next('A', 'Z');
            return input.Insert(index, charToAdd.ToString());
        }

        private string SwapCharactersAtRandom(string input, Random random)
        {
            if (input.Length < 2) return input;
            var index = random.Next(input.Length - 1);
            var chars = input.ToCharArray();
            (chars[index], chars[index + 1]) = (chars[index + 1], chars[index]);
            return new string(chars);
        }
    }
}
