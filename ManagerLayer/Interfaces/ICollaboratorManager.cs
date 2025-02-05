using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace ManagerLayer.Interfaces
{
    public interface ICollaboratorManager
    {
        public Collaborator AddCollaborator(AddCollaboratorModel model, int userId);
        public bool IsEmailExist(string email);
        public List<Collaborator> GetAllCollaborators(int userId);
        public bool DeleteCollaborator(string email, int notesId, int userId);
    }
}
