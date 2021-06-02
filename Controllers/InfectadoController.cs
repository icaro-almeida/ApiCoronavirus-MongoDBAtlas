using Api.Data.Collections;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfectadoController : ControllerBase
    {
        Data.MongoDB _mongoDB;
        IMongoCollection<Infectado> _infectadosCollection;

        public InfectadoController(Data.MongoDB mongoDB)
        {
            _mongoDB = mongoDB;
            _infectadosCollection = _mongoDB.DB.GetCollection<Infectado>(typeof(Infectado).Name.ToLower());
        }

        [HttpPost]
        public ActionResult SalvarInfectado([FromBody] InfectadoDto dto)
        {
            var infectado = new Infectado(dto.DataNascimento, dto.Sexo, dto.Latitude, dto.Longitude);

            _infectadosCollection.InsertOne(infectado);

            return StatusCode(201, "Infectado adicionado com sucesso");
        }

        [HttpGet]
        public ActionResult ObterInfectados()
        {
            var infectados = _infectadosCollection.Find(Builders<Infectado>.Filter.Empty).ToList();

            return Ok(infectados);
        }

        [HttpPut("{id}")]
        public ActionResult AtualizarInfectado(string id, [FromBody] InfectadoDto dto)
        {
            try
            {
                var filter = Builders<Infectado>.Filter.Eq("Id", ObjectId.Parse(id));
                //var update = Builders<BsonDocument>.Update.Set("class_id", 483);
                //_infectadosCollection.UpdateOne(Builders<Infectado>.Filter.Where(i => i.Id == ObjectId.Parse(id)), Builders<Infectado>.Update.Set("sexo", dto.Sexo));

                var infectado = new Infectado(dto.DataNascimento, dto.Sexo, dto.Latitude, dto.Longitude);
                infectado.Id = ObjectId.Parse(id).ToString();

                var replaceOneResult = _infectadosCollection.ReplaceOneAsync(
                    i => i.Id == ObjectId.Parse(id).ToString(),
                    infectado);

                return Ok("Atualizado com sucesso!");
            }
            catch
            {
                return StatusCode(400, "A atualização falhou!");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {

            _infectadosCollection.DeleteOne(Builders<Infectado>.Filter.Where(i => i.Id == ObjectId.Parse(id).ToString()));

            return StatusCode(200, "Excluído com sucesso!");
        }
    }
}
