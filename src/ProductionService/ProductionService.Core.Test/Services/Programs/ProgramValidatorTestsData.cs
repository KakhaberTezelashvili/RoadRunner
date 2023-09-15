using ProductionService.Shared.Dtos.Machines;
using ProductionService.Shared.Dtos.Programs;
using System.Collections;

namespace ProductionService.Core.Test.Services.Programs
{
    public class ProgramValidatorTestsData
    {
        public const int MachineKeyId = 1005;
        public const int ProgramKeyId = 1012;
        public const string ProgramName = "P1";
        public const string ProgramText = "Program 1";
    }

    public class ProgramForMachineValidateAsync_FailedTestData : IEnumerable<object[]>
    {
        private readonly List<object[]> data = new List<object[]>
        {
            /* Program doesn't exist */
            new object[] { null },
            /* Program isn't associated with any machine (Machines = null) */
            new object[] {
                new ProgramDetailsDto
                {
                    KeyId = ProgramValidatorTestsData.ProgramKeyId
                }
            },
            /* Program isn't associated with any machine (Machines.Count = 0) */
            new object[] {
                new ProgramDetailsDto
                {
                    KeyId = ProgramValidatorTestsData.ProgramKeyId,
                    Machines = new List<MachineDetailsBaseDto>()
                }
            },
            /* Program is associated with other machines, but not with needed machine */
            new object[] {
                new ProgramDetailsDto
                {
                    KeyId = ProgramValidatorTestsData.ProgramKeyId,
                    Machines = new List<MachineDetailsBaseDto>()
                    {
                        new MachineDetailsBaseDto() { KeyId = ProgramValidatorTestsData.MachineKeyId + 1 },
                        new MachineDetailsBaseDto() { KeyId = ProgramValidatorTestsData.MachineKeyId + 2 }
                    }
                }
            }
        };

        public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ProgramForMachineValidateAsync_SucceededTestData : IEnumerable<object[]>
    {
        private readonly List<object[]> data = new List<object[]>
            {
                /* Program is associated with needed machine only */
                new object[] {
                    new ProgramDetailsDto
                    {
                        KeyId = ProgramValidatorTestsData.ProgramKeyId,
                        Name = ProgramValidatorTestsData.ProgramName,
                        Text = ProgramValidatorTestsData.ProgramText,
                        Machines = new List<MachineDetailsBaseDto>()
                        {
                            new MachineDetailsBaseDto() { KeyId = ProgramValidatorTestsData.MachineKeyId }
                        }
                    }
                },
                /* Program is associated with needed and 2 other machines */
                new object[] {
                    new ProgramDetailsDto
                    {
                        KeyId = ProgramValidatorTestsData.ProgramKeyId,
                        Name = ProgramValidatorTestsData.ProgramName,
                        Text = ProgramValidatorTestsData.ProgramText,
                        Machines = new List<MachineDetailsBaseDto>()
                        {
                            new MachineDetailsBaseDto() { KeyId = ProgramValidatorTestsData.MachineKeyId },
                            new MachineDetailsBaseDto() { KeyId = ProgramValidatorTestsData.MachineKeyId + 1 },
                            new MachineDetailsBaseDto() { KeyId = ProgramValidatorTestsData.MachineKeyId + 2 }
                        }
                    }
                },
            };

        public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
