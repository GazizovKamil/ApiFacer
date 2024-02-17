using ApiFacer.DB;
using ConsoleFaiserScript.Classes;
using ConsoleFaiserScript.DB;
using System.Text.Json.Nodes;

namespace ConsoleFaiserScript.Requests
{
    public class dbRequest
    {
        public static async Task SaveUserImages(int personId, string yandexPath, string localPath, Person person, ApiDB dbContext, int eventId)
        {
            ApiDB db = dbContext;

            UserImages userImages = new UserImages()
            {
                userId = personId,
                yandexPath = yandexPath,
                localPath = localPath,
                eventId = eventId
            };

            await db.UserImages.AddAsync(userImages);
            await db.SaveChangesAsync();

            PersonVk personVk = new PersonVk()
            {
                userImagesId = userImages.id,
                tag = person.tag,
                coord = person.coord,
                confidence = person.confidence,
                awesomeness = person.awesomeness,
                similarity = person.similarity,
                sex = person.sex,
                emotion = person.emotion,
                age = person.age,
                valence = person.valence,
                arousal = person.arousal,
                frontality = person.frontality,
                visibility = person.visibility
            };

            await db.PersonVk.AddAsync(personVk);
            await db.SaveChangesAsync();
        }
    }
}
