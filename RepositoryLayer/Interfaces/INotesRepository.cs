using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interfaces
{
    public interface INotesRepository
    {
        public Notes AddNotes(AddNotesModel model, int userId);
        public List<Notes> GetAllNotes(int userId);
        public bool EditNotes(EditNotesModel model, int notesId, int userId);
        public bool DeleteNotes(int notesId);
        public bool PinUnPin(int userId, int notesId);
        public bool TrashUnTrash(int userId, int notesId);
        public bool Archive(int userId, int notesId);
        public bool DeleteForever(int userId, int notesId);
        public Notes Image(int userId, int notesId, IFormFile path);
        public Notes Remainder(int userId, int notesId, DateTime datetimeToReminder);
        public Notes Color(int userId, int notesId, string color);
    }
}
