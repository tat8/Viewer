using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viewer.Data;
using Viewer.Models;
using static Viewer.Enums.Enums.TopNodeType;

namespace Viewer.Services
{
    public static class GroupService
    {
        public static ObservableCollection<Node> Group(Enums.Enums.TopNodeType topNodeType=Login, int? year = null)
        {
            switch (topNodeType)
            {
                case Login:
                    return GroupByLogin(year);
                case SmObjectType:
                    return GroupBySmObjectType(year);
                default:
                    return GroupByLogin(year);
            }
        }

        /// <summary>
        /// Groups data with the structure 'Login'-'smObjectType'-'Operation'-'Date_Progect_ObjectId_LogText'
        /// </summary>
        /// <returns> ObservableCollection of structured data</returns>
        private static ObservableCollection<Node> GroupByLogin(int? year = null)
        {
            var protocols = DataService.GetProtocols(year);

            // Get all possible logins
            var logins = protocols.GroupBy(o => o.Login)
                    .Select(p => new { Name = p.Key })
                    .OrderBy(s => s.Name);

            var loginNodes = new ObservableCollection<Node>();

            foreach (var login in logins)
            {
                // Get records refered to current login
                var loginRecords = protocols.Where(o => o.Login == login.Name).ToList();

                // Get all possible smObject's types for current login
                var smObjectTypes = loginRecords.GroupBy(p => p.SmType)
                    .Select(g => new { Type = g.Key, Name = Translator.GetInstance().GetObjectName(g.Key - 1) })
                    .OrderBy(s => s.Name);

                var smObjectTypeNodes = new ObservableCollection<Node>();

                foreach (var smObjectType in smObjectTypes)
                {
                    // Records refered to current login and current smObjectType
                    var smObjectRecords = loginRecords.Where(o => o.SmType == smObjectType.Type).ToList();
                    
                    // Get subtree for operations and descriptions for current login and currtn smObjectType
                    var operationNodes = GroupByOperation(smObjectRecords, login.Name, smObjectType.Name,Login);

                    smObjectTypeNodes.Add(new Node
                    {
                        Name = smObjectType.Name,
                        Nodes = operationNodes,
                        NodePath = $"{login.Name}",
                        NodeStyle = StyleGetter.Get(1)
                    });
                }

                loginNodes.Add(new Node
                {
                    Name = login.Name,
                    Nodes = smObjectTypeNodes,
                    NodeStyle = StyleGetter.Get(0)
                });
            }

            return loginNodes;
        }

        private static ObservableCollection<Node> GroupBySmObjectType(int? year = null)
        {
            var protocols = DataService.GetProtocols(year);

            // Get all possible smObjectTypes
            var smObjectTypes = protocols.GroupBy(o => o.SmType)
                .Select(p => new {Type = p.Key, Name = Translator.GetInstance().GetObjectName(p.Key - 1)})
                .OrderBy(s => s.Name);

            var smObjectTypesNodes = new ObservableCollection<Node>();

            foreach (var smObjectType in smObjectTypes)
            {
                // Records refered to current smObjectType
                var smObjectRecords = protocols.Where(o => o.SmType == smObjectType.Type).ToList();

                // Get all possible logins for current cmObjectType
                var logins = smObjectRecords.GroupBy(o => o.Login)
                    .Select(p => new {Name = p.Key})
                    .OrderBy(s => s.Name);

                var loginNodes = new ObservableCollection<Node>();

                foreach (var login in logins)
                {
                    // Records refered to current smObjectType and current login
                    var loginRecords = smObjectRecords.Where(o => o.Login == login.Name).ToList();

                    // Get subtree for operations and descriptions for current login and currtn smObjectType
                    var operationNodes = GroupByOperation(loginRecords, login.Name, smObjectType.Name, SmObjectType);

                    loginNodes.Add(new Node
                    {
                        Name = login.Name,
                        Nodes = operationNodes,
                        NodePath = $"{smObjectType.Name}",
                        NodeStyle = StyleGetter.Get(1)
                    });
                }

                smObjectTypesNodes.Add(new Node
                {
                    Name = smObjectType.Name,
                    Nodes = loginNodes,
                    NodeStyle = StyleGetter.Get(0)
                });
            }

            return smObjectTypesNodes;
        }


        private static ObservableCollection<Node> GroupByOperation(List<A0Protocol> protocol, string login, string smObjectType, Enums.Enums.TopNodeType topNodeType)
        {
            // Get all possible operation's types for current login and current smObjectType
            var operationTypes = protocol.GroupBy(g => g.Oper)
                .Select(u => new { Type = u.Key, Name = Translator.GetInstance().GetOperationName(u.Key - 1) })
                .OrderBy(s => s.Name);

            var operationNodes = new ObservableCollection<Node>();

            foreach (var operationType in operationTypes)
            {
                // Records refered to current login, current smObjectType and current operationType
                var operationRecords = protocol.Where(o => o.Oper == operationType.Type);

                // Get all descriptions for current login, current smObjectType and current operationType
                var descriptionItems = operationRecords.Select(o => new
                        { EvDate = o.EvDate, ProjID = o.ProjID, SmObjID = o.SmObjID, LogText = o.LogText })
                    .OrderBy(p => p.EvDate);

                var descriptionNodes = new ObservableCollection<Node>();

                foreach (var descriptionItem in descriptionItems)
                {
                    var descriptionNode = new DescriptionNode(descriptionItem.EvDate, descriptionItem.ProjID,
                        descriptionItem.SmObjID, descriptionItem.LogText);

                    // Create path according which node is the first
                    switch (topNodeType)
                    {
                        case Login:
                            descriptionNode.NodePath = $"{login}/{smObjectType}/{operationType.Name}";
                            break;
                        case SmObjectType:
                            descriptionNode.NodePath = $"{smObjectType}/{login}/{operationType.Name}";
                            break;
                        default:
                            descriptionNode.NodePath = $"{login}/{smObjectType}/{operationType.Name}";
                            break;
                    }

                    descriptionNode.NodeStyle = StyleGetter.Get(3);

                    descriptionNodes.Add(descriptionNode);
                }

                var operationNode = new Node
                {
                    Name = operationType.Name,
                    Nodes = descriptionNodes,
                    NodeStyle = StyleGetter.Get(2)
                };

                // Create path according which node is the first
                switch (topNodeType)
                {
                    case Login:
                        operationNode.NodePath = $"{login}/{smObjectType}";
                        break;
                    case SmObjectType:
                        operationNode.NodePath = $"{smObjectType}/{login}";
                        break;
                    default:
                        operationNode.NodePath = $"{login}/{smObjectType}";
                        break;
                }

                operationNodes.Add(operationNode);
            }

            return operationNodes;
        }
    }
}
