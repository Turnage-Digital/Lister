using Lister.Core.ValueObjects;
using Lister.Services.OpenAi;
using MediatR;

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
    private readonly IOpenAIService _openAiService;

    public ConvertTextToListItemCommandHandler(IOpenAIService openAiService)
    {
        _openAiService = openAiService;
    }

    public async Task<Item> Handle(ConvertTextToListItemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}