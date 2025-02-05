using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Migrations;

namespace ManagerLayer.Services
{
    public class NotesManager:INotesManager
    {

        private readonly INotesRepository notes;
        private readonly FundooDBContext context;

        public NotesManager(INotesRepository notes, FundooDBContext context)
        {
            this.notes = notes;
            this.context = context;
        }

        public Notes AddNotes(AddNotesModel model, int userId)
        {
            return notes.AddNotes(model,userId);
        }
        public List<Notes> GetAllNotes(int userId)
        {
            return notes.GetAllNotes(userId);
        }

        

        public bool EditNotes(EditNotesModel model, int notesId, int userId)
        {
            return notes.EditNotes(model, notesId, userId);
        }
        public bool DeleteNotes(int notesId)
        {
            return notes.DeleteNotes(notesId);
        }
        public bool PinUnPin(int userId, int notesId)
        {
            return notes.PinUnPin(userId, notesId);
        }
        public bool TrashUnTrash(int userId, int notesId)
        {
            return notes.TrashUnTrash(userId, notesId);

        }
        public bool Archive(int userId, int notesId)
        {
            return notes.Archive(userId, notesId);
        }
        public bool DeleteForever(int userId, int notesId)
        {
            return notes.DeleteForever(userId, notesId);
        }
        public Notes Image(int userId, int notesId, IFormFile path)
        {
            return notes.Image(userId, notesId, path);

        }
        public Notes Remainder(int userId, int notesId, DateTime datetimeToReminder)
        {
            return notes.Remainder(userId, notesId, datetimeToReminder);
        }
        public Notes Color(int userId, int notesId, string color)
        {
            return notes.Color(userId, notesId, color);
        }


    }
}
