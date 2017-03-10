using System;
using System.Collections.Generic;
using ConsoleApplication.Messages;

namespace ConsoleApplication
{
    public static class Users
    {
        public static List<User> users
        {
            get
            {
                var u1 = new User
                {
                    Id = 1,
                    FirstName = "Alexis",
                    LastName = "Alulema",
                    Birthday = new DateTime(1975, 11, 22).Ticks
                };

                u1.Vehicles.Add(new Vehicle
                {
                    Id = 1,
                    RegNumber = "HI2938AWKHQDB"
                });
                u1.Vehicles.Add(new Vehicle()
                {
                    Id = 2,
                    RegNumber = "JQ0289DNCJSKL"
                });

                var u2 = new User
                {
                    Id = 2,
                    FirstName = "Steve",
                    LastName = "Jobs",
                    Birthday = new DateTime(1978, 4, 21).Ticks,
                };

                u2.Vehicles.Add(new Vehicle()
                {
                    Id = 3,
                    RegNumber = "UDJYSBH1928JS"
                });
                u2.Vehicles.Add(new Vehicle()
                {
                    Id = 4,
                    RegNumber = "NNKKSUD02JKSL"
                });

                var u3 = new User
                {
                    Id = 3,
                    FirstName = "John",
                    LastName = "Williams",
                    Birthday = new DateTime(2010, 9, 22).Ticks,
                };

                u3.Vehicles.Add(new Vehicle()
                {
                    Id = 5,
                    RegNumber = "PSKSL239DJ30D"
                });
                u3.Vehicles.Add(new Vehicle()
                {
                    Id = 6,
                    RegNumber = "KJSHKASHIHXNI"
                });

                return new List<User>
                { u1, u2, u3 };
            }
        }
    }
}
