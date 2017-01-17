using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotesSingle
{
    public static class NoteDatabase
	{
		public static  List<Note> GetNotesFromDatabase()
		{
			try
			{
				var client = new RestClient("http://ndraz.com:/NotesApi/");
				var request = new RestRequest(Method.POST);
				request.AddHeader("postman-token", "38bf6ae3-a903-878e-6eca-24c47100201d");
				request.AddHeader("cache-control", "no-cache");
				request.AddHeader("content-type", "application/json");
				request.AddParameter("application/json", "{\n  \"action\": \"getall\"\n\n}", ParameterType.RequestBody);

				var response = client.Execute(request);
				//Console.WriteLine(response.Content);
				
				var start = response.Content.IndexOf('[');
				var end = response.Content.IndexOf(']');
				if (start < 0 || end < 0)
				{
					Console.WriteLine("RestSharp Error Message: " + response.ErrorException.Message);
					//Sometimes the failure is just a weird bug (supposedly in Mono) and not a problem with the call
					//Try again and see what happens
					response = client.Execute(request);
					start = response.Content.IndexOf('[');
					end = response.Content.IndexOf(']');
				}

				var result = response.Content.Substring(start, end - start + 1);
				var notes = JsonConvert.DeserializeObject<List<Note>>(result);
				return notes;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in Get All: " + ex.Message);
				return new List<Note>(); //return an empty list so the app doesn't crash
			}
		}

		public static Note GetNoteById(long id)
		{
			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "25ed98cf-d91a-7c8f-660b-eb5152d417dc");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n\t\"action\": \"getid\",\n\t\"Id\": \"" + id + "\"\n}", ParameterType.RequestBody);
			var response = client.Execute(request);

			var start = response.Content.IndexOf('[');
			var end = response.Content.IndexOf(']');
			if (start < 0 || end < 0)
			{
				Console.WriteLine("RestSharp Error Message: " + response.ErrorException.Message);
				//Sometimes the failure is just a weird bug (supposedly in Mono) and not a problem with the call
				//Try again and see what happens
				response = client.Execute(request);
				start = response.Content.IndexOf('[');
				end = response.Content.IndexOf(']');
			}

			try
			{
				var result = response.Content.Substring(start, end - start + 1);
				var note = JsonConvert.DeserializeObject<List<Note>>(result);
				//Console.WriteLine(note[0].Id);
				return note[0];
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in Get By ID: " + ex.Message);
				return null;
			}

		}

		public static async Task UpdateNote(Note note)
		{
			var id = note.Id;
			var title = note.Title;
			var content = note.Content;

			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "64c0f8d8-9b02-4e78-e052-bdc88799aae5");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n\t\"action\": \"update\",\n\t\"Id\": \"" + id + "\",\n\t\"Title\": \"" + EscapeForJson(title) + "\",\n\t\"Content\": \"" + EscapeForJson(content) + "\"\n}", ParameterType.RequestBody);
			await client.ExecuteTaskAsync(request);

			//Console.WriteLine(response.Content);
		}

		public static async Task<Note> InsertNote(string title, string content)
		{
			var client = new RestClient("http://ndraz.com/NotesApi/");
			var request = new RestRequest(Method.POST);
			request.AddHeader("postman-token", "5b086c62-b3f2-d68b-eeb4-660251ce9b6e");
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/json");
			request.AddParameter("application/json", "{\n\t\"action\": \"add\",\n\t\"Title\": \"" + EscapeForJson(title) + "\",\n\t\"Content\": \"" + EscapeForJson(content) + "\",\n\t\"User\": \"1\"\n}", ParameterType.RequestBody);
			var response = await client.ExecuteTaskAsync(request);

			if (response.Content == null || response.Content.Equals(""))
			{
				response = await client.ExecuteTaskAsync(request);
			}

			var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

			return new Note()
			{
				Id = Convert.ToInt64(obj["InsertedId"]),
				Title = title,
				Content = content
			};

		}

		public static string EscapeForJson(string input)
		{
			return input.Replace("\n", "\\n").Replace("\t", "\\t").Replace("\b", "\\b").Replace("\f", "\\f").Replace("\r", "\\r");
		}
	}
}
