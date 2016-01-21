using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.Site
{
    public class SiteIndexerHandler:ATaskHandler
    {
        public SiteIndexerHandler(TaskParameters taskParameters) : base(taskParameters) { }



        public override bool Handle()
        {
            var siteGroups = TaskParameters.Context.ShSITEs.Where(s => !s.Index.HasValue).GroupBy(g => g.MacroRegion);
            List<SiteIndexerImportModel> import = new List<SiteIndexerImportModel>();
            foreach (var siteGroup in siteGroups)
            {
                var _index = siteGroup.Max(i => i.Index);
                int index = 0;
                if (!_index.HasValue  )
                {
                    if (siteGroup.Key == "Ural")
                        index = 1000000;
                    else
                        if (siteGroup.Key == "Siberia")
                            index = 3000000;
                        else
                            continue;

                }
                else
                    index=_index.Value;

                foreach (var site  in siteGroup)
                {
                    index++;
                    import.Add(new SiteIndexerImportModel() { Site=site.Site, Index = index });
                }

               
            }
            if (import.Count() > 0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(import) });
            }
            return true;
        }
        public class SiteIndexerImportModel
        {
            public string Site { get; set; }
            public int Index { get; set; }
        }
    }
}
