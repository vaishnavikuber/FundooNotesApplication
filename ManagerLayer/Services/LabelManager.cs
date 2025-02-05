using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interfaces;
using RepositoryLayer.Entity;
using RepositoryLayer.Interfaces;

namespace ManagerLayer.Services
{
    public class LabelManager:ILabelManager
    {
        private readonly ILabelRepository label;

        public LabelManager(ILabelRepository label)
        {
            this.label = label;
        }
        public Label AddLabel(LabelModel model, int userId)
        {
            return label.AddLabel(model, userId);
        }
        public List<Label> GetAllLabels(int userId)
        {
            return label.GetAllLabels(userId);
        }
        public bool DeleteLabel(int userId, int labelId)
        {
            return label.DeleteLabel(userId,  labelId);
        }
        public bool UpdateLabel(int userId, int labelId, string labelName)
        {
            return label.UpdateLabel(userId, labelId, labelName);
        }
    }
}
