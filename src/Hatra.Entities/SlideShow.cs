﻿using Hatra.Entities.AuditableEntity;

namespace Hatra.Entities
{
    public class SlideShow : IAuditableEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string BriefDescription { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string Link { get; set; }

        public int Order { get; set; }
    }
}
