using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interfaces
{
    public interface ICollaboratorRepository
    {
        public Collaborator AddCollaborator(AddCollaboratorModel model, int userId);
        public List<Collaborator> GetAllCollaborators(int userId);
        public bool DeleteCollaborator(string email, int notesId, int userId);
    }
}
