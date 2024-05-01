﻿using Core.Monads;

namespace Core.Internet.Http;

public static class HttpClientExtensions
{
   public static async Task<Completion<string>> StringAsync(this HttpClient httpClient, string url, CancellationToken token)
   {
      try
      {
         var response = await httpClient.GetAsync(url);
         response.EnsureSuccessStatusCode();

         return (await response.Content.ReadAsStringAsync()).Completed(token);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}