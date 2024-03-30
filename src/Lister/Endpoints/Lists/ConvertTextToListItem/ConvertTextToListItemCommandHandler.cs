using System.Diagnostics;
using System.Dynamic;
using Lister.Core.Enums;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Views;
using Lister.Core.ValueObjects;
using Lister.Services.OpenAI;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Lister.Endpoints.Lists.ConvertTextToListItem;

/*

 import OpenAI from "openai";

   const openai = new OpenAI({
       organization: '',
       apiKey: ''
   });

   const example = '{"city": "Spring Hill", "name": "Ricky Lafluer", "state": "FL", "status": "Active", "address": "100 Maple Lane", "zipCode": "34609", "dateOfBirth": "1970-01-01T05:00:00.0000000Z"}'

   async function main() {
       const completion = await openai.chat.completions.create({
           messages: [
               {role: "system", content: "Here is an example of a JSON object: " + example},
               {role: "user", content: "Can you parse the following email text into that object's structure?"},
               {
                   role: "user",
                   content: "Hello Erika, I got the info for the new patient, his name is Heath Turnage and he's from Orlando. " +
                       "His address is 1234 Elm Street, and his zip code is 32801. His dob is 11/08/1980. He's currently active. Thanks!"
               }
           ],
           model: "gpt-3.5-turbo",
       });

       console.log(completion.choices[0]);
   }

   main();

 */

public class ConvertTextToListItemCommandHandler : IRequestHandler<ConvertTextToListItemCommand, Item>
{
    private readonly ListerDbContext _dbContext;
    private readonly IOpenAIService _openAiService;

    public ConvertTextToListItemCommandHandler(ListerDbContext dbContext, IOpenAIService openAiService)
    {
        _dbContext = dbContext;
        _openAiService = openAiService;
    }

    public async Task<Item> Handle(ConvertTextToListItemCommand request, CancellationToken cancellationToken)
    {
        var parsed = Guid.Parse(request.ListId);
        var retval = await _dbContext.Lists
            .Where(list => list.CreatedBy == request.UserId)
            .Where(list => list.Id == parsed)
            .Select(list => new ListItemDefinitionView
            {
                Id = list.Id,
                Name = list.Name,
                Columns = list.Columns
                    .Select(column => new Column
                    {
                        Name = column.Name,
                        Type = column.Type
                    }).ToArray(),
                Statuses = list.Statuses
                    .Select(status => new Status
                    {
                        Name = status.Name,
                        Color = status.Color
                    }).ToArray()
            })
            .AsSplitQuery()
            .SingleAsync(cancellationToken);
        
        // make a dynamic object from the ListItemDefinitionView then convert it to a JSON string
        dynamic example = new ExpandoObject();
        foreach (var column in retval.Columns)
        {
            object value =  column.Type switch
            {
                ColumnType.Text => "string",
                ColumnType.Number => 1,
                ColumnType.Date => DateTime.UtcNow.ToString("o"),
                ColumnType.Boolean => false
            };
            ((IDictionary<string, object?>)example)[column.Property] = value;
        }

        var serialized = JsonConvert.SerializeObject(example);
        
        // var completion = await _openAiService.CreateCompletion(new OpenAIRequest
        // {
        //     Messages = new List<OpenAIMessage>
        //     {
        //         new OpenAIMessage
        //         {
        //             Role = "system",
        //             Content = "Here is an example of a JSON object: " + serialized
        //         },
        //         new OpenAIMessage
        //         {
        //             Role = "user",
        //             Content = "Can you parse the following email text into that object's structure?"
        //         },
        //         new OpenAIMessage
        //         {
        //             Role = "user",
        //             Content = request.Text
        //         }
        //     },
        //     Model = "gpt-3.5-turbo"
        // });
        
        throw new NotImplementedException();
    }
}