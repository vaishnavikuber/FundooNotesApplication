using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Migrations;

namespace RepositoryLayer.Services
{
    public class NotesRepository : INotesRepository
    {
        private readonly FundooDBContext context;

        public NotesRepository(FundooDBContext context)
        {
            this.context = context;
        }

        public Notes AddNotes(AddNotesModel model, int userId)
        {
            Notes notes = new Notes();
            notes.Title = model.Title;
            notes.Description = model.Description;
            notes.UserID = userId;
            notes.CreatedAt = DateTime.Now;
            context.Notes.Add(notes);
            context.SaveChanges();
            return notes;
        }

        public List<Notes> GetAllNotes(int userId)
        {
            List<Notes> noteList = new List<Notes>();
            var notes = context.Notes.Select(x => x).Where(x => x.UserID == userId);

            if (notes != null)
            {
                foreach (var note in notes)
                {
                    noteList.Add(note);
                }
                return noteList;
            }
            return null;
        }

        public bool EditNotes(EditNotesModel model,int notesId, int userId)
        {
            GetAllNotes(userId);
            Notes notes = context.Notes.FirstOrDefault(x => x.UserID == userId && x.NotesID==notesId);
            if (notes != null)
            {
                notes.Title = model.Title;
                notes.Description = model.Description;
                notes.Reminder = model.Reminder;
                notes.Color = model.Color;
                notes.Image = model.Image;
                notes.IsArchive = model.IsArchive;
                notes.IsPin = model.IsPin;
                notes.IsTrash = model.IsTrash;
                notes.UpdatedAt = DateTime.Now;
                context.Update(notes);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteNotes(int notesId)
        {
            Notes note = context.Notes.FirstOrDefault(x => x.NotesID == notesId);
            if (note != null)
            {
                context.Notes.Remove(note);
                context.SaveChanges();
                return true;
            }
            return false;

        }

    }
}
