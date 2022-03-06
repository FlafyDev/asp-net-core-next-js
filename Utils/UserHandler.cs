using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Boxing
{
	public static class UserHandler
	{
		public enum Gender
		{
			Male,
			Female,
			Other,
		}

		public enum BoxingLevel
		{
			Beginner,
			Intermediate,
			Expert,
		}


		public class User
		{
			public int Id { get; }
			public string Email { get; set; }
			public string Username { get; set; }
			public string Password { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public int Age { get; set; }
			public Gender Gender { get; set; }
			public int BoxingPoints { get; set; }
			public bool Admin { get; set; }

			public BoxingLevel BoxingLevel
			{
				get
				{
					switch (BoxingPoints)
					{
						case int x when (x >= 100):
							return BoxingLevel.Intermediate;
						case int x when (x >= 200):
							return BoxingLevel.Expert;
						default:
							return BoxingLevel.Beginner;
					}
				}
			}

			public User(int id)
			{
				Id = id;
			}

			//public User(
			//	int id,
			//	string username,
			//	string email,
			//	string password,
			//	string firstName,
			//	string lastName,
			//	int age,
			//	Gender gender,
			//	int boxingPoints
			//)
			//{
			//	Id = id;
			//	Username = username;
			//	Email = email;
			//	Password = password;
			//	FirstName = firstName;
			//	LastName = lastName;
			//	Age = age;
			//	Gender = gender;
			//	BoxingPoints = boxingPoints;
			//}
		}

		public static User GetUserByUsername(string username)
		{
			return GetUser($@"
				select * from Users where Username=N'{username}'
			");
		}

		public static User[] GetUsers(string query)
		{
			var data = SQLHelper.SelectData(query);
			var users = new User[data.Rows.Count];
			for (int i = 0; i < users.Length; i++)
			{
				users[i] = new User((int)data.Rows[i].ItemArray[0])
				{
					Username =		 (string)data.Rows[i].ItemArray[1],
					Email =				 (string)data.Rows[i].ItemArray[2],
					Password =		 (string)data.Rows[i].ItemArray[3],
					FirstName =		 (string)data.Rows[i].ItemArray[4],
					LastName =		 (string)data.Rows[i].ItemArray[5],
					Age =							(int)data.Rows[i].ItemArray[6],
					Gender =			 (Gender)data.Rows[i].ItemArray[7],
					BoxingPoints =		(int)data.Rows[i].ItemArray[8],
					Admin =					 (bool)data.Rows[i].ItemArray[9],
				};
			}

			return users;
		}

		public static User GetUser(string query)
		{
			var users = GetUsers(query);
			return users.Length == 1 ? users[0] : null;
		}

		public static bool DeleteUser(int userId) {
			return (SQLHelper.DoQuery($@"
				delete from Users
				where Id='{userId}'
			") == 1);
		}

		public static bool UpdateUser(User user)
		{
			return (SQLHelper.DoQuery($@"
				Update Users
				Set Username = N'{user.Username}',
					Email = N'{user.Email}', 
					Password = N'{user.Password}',
					FirstName = N'{user.FirstName}',
					LastName = N'{user.LastName}',
					Age = {user.Age},
					Gender = {(int)user.Gender},
					Admin = {(user.Admin ? 1 : 0)}
				Where Id = {user.Id}"
			) == 1);
		}

		public static User RegisterUser(User user, out string error)
		{
			try
			{
				// Check if input is valid.
				error = ValidateUser(user);
				if (error != "") return null;

				// Check if a user already exists with the same email/username.
				if (GetUser($@"
				select * from Users where Email=N'{user.Email}' or Username=N'{user.Username}'
			") != null)
				{
					error = "Email or username is already taken";
					return null;
				}

				// Add the user to the database.
				if (SQLHelper.DoQuery($@"
				INSERT INTO Users (Username, Email, Password, FirstName, LastName, Age, Gender)
				VALUES (N'{user.Username}', N'{user.Email}', N'{user.Password}', N'{user.FirstName}', N'{user.LastName}', {user.Age}, {(int)user.Gender});"
				) == 1)
				{
					return GetUserByUsername(user.Username);
				}
				else
				{
					error = "Internal Error";
					return null;
				}
			} catch (Exception e)
			{
				error = e.Message;
				return null;
			}
		}

		static Regex emailRegex = new Regex(@"^\S+@\S+\.\S+$");
		static Regex usernameRegex = new Regex(@"^[A-Z,a-z]{3,12}$");
		static Regex passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
		public static string ValidateUser(User user)
		{
			if (!emailRegex.IsMatch(user.Email))
			{
				return "Email must be a real email.";
			}
			else if (!passwordRegex.IsMatch(user.Password))
			{
				return "Password must contain a minimum of 8 characters, 1 letter and 1 number";
			}
			else if (!usernameRegex.IsMatch(user.Username))
			{
				return "Username must be between 3-12 english letters";
			}
			else
			{
				return "";
			}
		}
	}
}