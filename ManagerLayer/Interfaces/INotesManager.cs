using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace ManagerLayer.Interfaces
{
    public interface INotesManager
    {
        public Notes AddNotes(AddNotesModel model, int userId);
        public List<Notes> GetAllNotes(int userId);
        public bool EditNotes(EditNotesModel model, int notesId, int userId);
        public bool DeleteNotes(int notesId);
        //public EditNotesModel Model(int notesId);
    }
}
