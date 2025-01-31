using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
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

        //public EditNotesModel Model(int notesId)
        //{
        //    Notes note = context.Notes.FirstOrDefault(x => x.NotesID == notesId);
        //    if (note == null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        EditNotesModel model = new EditNotesModel();
        //        model.Title = note.Title;
        //        model.Description = note.Description;
        //        model.Reminder = note.Reminder;
        //        model.IsArchive = note.IsArchive;
        //        model.Color = note.Color;
        //        model.IsPin = note.IsPin;
        //        model.Image = note.Image;
        //        model.IsTrash = note.IsTrash;
        //        model.UpdatedAt = DateTime.Now;
        //        return model;
        //    }
        //}

        public bool EditNotes(EditNotesModel model, int notesId, int userId)
        {
            return notes.EditNotes(model, notesId, userId);
        }
        public bool DeleteNotes(int notesId)
        {
            return notes.DeleteNotes(notesId);
        }



    }
}
