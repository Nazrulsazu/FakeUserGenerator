using Bogus;
using System;
using FakeUserGenerator.Models;

namespace FakeUserGenerator.Services
{
    public class FakeUserService
    {
        public List<FakeUser> GenerateUserData(string region, int errorCount, int seed, int page, int count = 20)
        {
            var random = new Random(seed + page);  // Seed based on user seed and page
            var users = new List<FakeUser>();

            for (int i = 0; i < count; i++)
            {
                var user = new FakeUser
                {
                    Index = i + 1 + (page * count),  // Page based index
                    RandomId = Guid.NewGuid().ToString(),
                    Name = GenerateName(region, random),
                    Address = GenerateAddress(region, random),
                    Phone = GeneratePhone(region, random)
                };

                ApplyErrors(user, errorCount, random);
                users.Add(user);
            }

            return users;
        }

        private string GenerateName(string region, Random random)
        {
            var name = region switch
            {
                "Poland" => new Faker("pl").Name.FullName(),
                "USA" => new Faker("en").Name.FullName(),
                "Georgia" => new Faker("ka").Name.FullName(),
                _ => new Faker().Name.FullName()
            };
            return name;
        }

        private string GenerateAddress(string region, Random random)
        {
            var address = region switch
            {
                "Poland" => new Faker("pl").Address.FullAddress(),
                "USA" => new Faker("en").Address.FullAddress(),
                "Georgia" => new Faker("ka").Address.FullAddress(),
                _ => new Faker().Address.FullAddress()
            };
            return address;
        }

        private string GeneratePhone(string region, Random random)
        {
            var phone = region switch
            {
                "Poland" => new Faker("pl").Phone.PhoneNumber(),
                "USA" => new Faker("en").Phone.PhoneNumber(),
                "Georgia" => new Faker("ka").Phone.PhoneNumber(),
                _ => new Faker().Phone.PhoneNumber()
            };
            return phone;
        }

        private void ApplyErrors(FakeUser user, int errorCount, Random random)
        {
            if (errorCount <= 0) return;

            // Apply error logic (delete, add, swap)
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
                case 0:  // Delete character
                    selectedField = DeleteCharacterAtRandom(selectedField, random);
                    break;
                case 1:  // Add character
                    selectedField = AddCharacterAtRandom(selectedField, random);
                    break;
                case 2:  // Swap characters
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
