using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;

namespace RepositoryLayer.Services
{
    public class CollaboratorRepository:ICollaboratorRepository
    {
        private readonly FundooDBContext context;
        public CollaboratorRepository(FundooDBContext context)
        {
            this.context = context;
        }

        
        public Collaborator AddCollaborator(AddCollaboratorModel model,int userId)
        {
            Collaborator collaborator=new Collaborator();
            collaborator.Email = model.Email;
            collaborator.NotesID = model.NotesID;
            collaborator.UserId = userId;
            context.Collaborators.Add(collaborator);
            context.SaveChanges();
            return collaborator;
        }

        public List<Collaborator> GetAllCollaborators(int userId)
        {
            var collaboratorList=context.Collaborators.Select(x=>x).Where(x=>x.UserId == userId).ToList();
            if (collaboratorList != null)
            {
                return collaboratorList;
            }
            return null;
        }


        public bool DeleteCollaborator(string email, int notesId, int userId)
        {
            var collaborator=context.Collaborators.FirstOrDefault(x=>x.Email == email && x.NotesID==notesId && x.UserId==userId);
            if(collaborator != null)
            {
                context.Collaborators.Remove(collaborator);
                return true;
            }
            return false;
        }
    }
}
