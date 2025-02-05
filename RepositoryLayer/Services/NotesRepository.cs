using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CommonLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Migrations;

namespace RepositoryLayer.Services
{
    public class NotesRepository : INotesRepository
    {
        private readonly FundooDBContext context;
        private readonly IConfiguration configuration;

        public NotesRepository(FundooDBContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
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

        public bool PinUnPin(int userId,int notesId)
        {
            var checkPin = context.Notes.FirstOrDefault(x => x.NotesID == notesId && x.UserID == userId);
            if (checkPin != null)
            {
                if (checkPin.IsPin == true)
                {
                    checkPin.IsPin = false;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    checkPin.IsPin = true;
                    context.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public bool TrashUnTrash(int userId, int notesId)
        {
            var checkPin = context.Notes.FirstOrDefault(x => x.NotesID == notesId && x.UserID == userId);
            if (checkPin != null)
            {
                if (checkPin.IsTrash == true)
                {
                    checkPin.IsTrash = false;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    checkPin.IsTrash = true;
                    context.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }


        public bool Archive(int userId, int notesId)
        {
            var checkPin = context.Notes.FirstOrDefault(x => x.NotesID == notesId && x.UserID == userId);
            if (checkPin != null)
            {
                if (checkPin.IsArchive == true)
                {
                    checkPin.IsArchive = false;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    checkPin.IsArchive = true;
                    context.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public bool DeleteForever(int userId,int notesId)
        {
            try
            {
                var notes = context.Notes.FirstOrDefault(x => x.NotesID == notesId && x.UserID == userId);
                if (notes != null)
                {
                    context.Notes.Remove(notes);
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public Notes Image(int userId, int notesId,IFormFile path)
        {
            try
            {
                var data = context.Notes.FirstOrDefault(x => x.UserID == userId && x.NotesID == notesId);
                if (data != null)
                {

                    data.Image = UploadImage(path, notesId, userId);
                    data.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                   // UploadImage(path, notesId, userId);
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Notes Remainder(int userId,int notesId,DateTime datetimeToReminder)
        {
            try
            {
                var data = context.Notes.FirstOrDefault(x => x.UserID == userId && x.NotesID == notesId);
                if (data != null)
                {
                    data.Reminder = datetimeToReminder;
                    data.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Notes Color(int userId,int notesId,string color)
        {
            try
            {
                var data = context.Notes.FirstOrDefault(x => x.UserID == userId && x.NotesID == notesId);
                if (data != null)
                {
                    data.Color = color;
                    data.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Notes GetNotesById(int userId,int notesId)
        {
            Notes note = context.Notes.FirstOrDefault(x => x.UserID == userId && x.NotesID == notesId);
            if(note != null)
            {
                return note;
            }
            return null;
        }

        public string UploadImage(IFormFile imagePath,int notesId,int userId)
        {
            var user = context.Users.Any(x=>x.UserID == userId);
            if (user)
            {
                var note = GetNotesById(userId, notesId);
                if(note != null)
                {
                    Account account = new Account(configuration["Cloudinary:CloudName"], configuration["Cloudinary:ApiKey"], configuration["Cloudinary:ApiSecret"]);
                    Cloudinary cloudinary = new Cloudinary(account);
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imagePath.FileName, imagePath.OpenReadStream()),
                        PublicId = note.Title
                    };
                    var uploadImage=cloudinary.Upload(uploadParams);
                    if (uploadImage != null)
                    {
                        note.UpdatedAt = DateTime.Now;
                        note.Image=uploadImage.Url.ToString();
                        context.SaveChanges();
                        return uploadImage.Url.ToString();
                    }
                    
                }
                
            }
            return null;
        }


    }
}
