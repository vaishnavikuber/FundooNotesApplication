using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;

namespace ManagerLayer.Services
{
    public class CollaboratorManager:ICollaboratorManager
    {
        private readonly FundooDBContext context;
        private readonly ICollaboratorRepository collaborator;
        public CollaboratorManager(FundooDBContext context, ICollaboratorRepository collaborator)
        {
            this.context = context;
            this.collaborator = collaborator;
        }

        public Collaborator AddCollaborator(AddCollaboratorModel model, int userId)
        {
            return collaborator.AddCollaborator(model, userId);
        }

        public List<Collaborator> GetAllCollaborators(int userId)
        {
           return collaborator.GetAllCollaborators(userId);
        }

        public bool DeleteCollaborator(string email, int notesId, int userId)
        {
            return collaborator.DeleteCollaborator(email, notesId, userId);
        }
        public bool IsEmailExist(string email)
        {
            var result = context.Users.FirstOrDefault(x => x.Email == email);
            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}
