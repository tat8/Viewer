using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Viewer.Data;
using Viewer.Enums.TreeEnums;
using Viewer.Models;

namespace Viewer.Services
{
    /// <summary>
    /// Groups data of 'A0Protocol' structure as the tree structure
    /// </summary>
    public static class GroupService
    {
        /// <summary>
        /// Groups data from table 'A0Protocol' as a tree structure
        /// </summary>
        /// <param name="topNodeEnum"> type of top node </param>
        /// <param name="year"> year to filter records </param>
        /// <returns> structured data from 'A0Protocol' table as a tree </returns>
        public static ObservableCollection<Node> Group(TopNodeEnum topNodeEnum=TopNodeEnum.Login, int? year = null)
        {
            switch (topNodeEnum)
            {
                case TopNodeEnum.Login:
                    return GroupByLogin(year);
                case TopNodeEnum.SmObjectType:
                    return GroupBySmObjectType(year);
                default:
                    return GroupByLogin(year);
            }
        }

        /// <summary>
        /// Groups data with the structure 'Login'-'SmObjectType'-'Operation'-'Date_Progect_ObjectId_LogText'
        /// </summary>
        /// <param name="year"> year to filter data </param>
        /// <returns> structured data as a tree </returns>
        private static ObservableCollection<Node> GroupByLogin(int? year = null)
        {
            var protocols = DataService.GetProtocols(year).ToList();

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
                    .Select(g => new { Type = g.Key, Name = Convertor.GetInstance().GetObjectName(g.Key - 1) })
                    .OrderBy(s => s.Name);

                var smObjectTypeNodes = new ObservableCollection<Node>();

                foreach (var smObjectType in smObjectTypes)
                {
                    // Records refered to current login and current smObjectType
                    var smObjectRecords = loginRecords.Where(o => o.SmType == smObjectType.Type).ToList();
                    
                    // Get subtree for operations and descriptions for current login and currtn smObjectType
                    var operationNodes = GroupByOperation(smObjectRecords, login.Name, smObjectType.Name, TopNodeEnum.Login);

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

        /// <summary>
        /// Groups data with the structure 'SmObjectType'-'Login'-'Operation'-'Date_Progect_ObjectId_LogText'
        /// </summary>
        /// <param name="year"> year to filter data </param>
        /// <returns> structured data as a tree </returns>
        private static ObservableCollection<Node> GroupBySmObjectType(int? year = null)
        {
            var protocols = DataService.GetProtocols(year).ToList();

            // Get all possible smObjectTypes
            var smObjectTypes = protocols.GroupBy(o => o.SmType)
                .Select(p => new {Type = p.Key, Name = Convertor.GetInstance().GetObjectName(p.Key - 1)})
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
                    var operationNodes = GroupByOperation(loginRecords, login.Name, smObjectType.Name, TopNodeEnum.SmObjectType);

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

        /// <summary>
        /// Builds an inner part of the tree for A0Protocol structure: 'Operation'-'Date_Progect_ObjectId_LogText'
        /// </summary>
        /// <param name="protocol"> data to be structured </param>
        /// <param name="login"> name of (grand)parent node 'Login' </param>
        /// <param name="smObjectType"> name of (grand)parent node 'SmObjectType' </param>
        /// <param name="topNodeEnum"> type of top node </param>
        /// <returns> structured data as an inner part of a tree </returns>
        private static ObservableCollection<Node> GroupByOperation(List<A0Protocol> protocol, string login, string smObjectType, TopNodeEnum topNodeEnum)
        {
            // Get all possible operation's types for current login and current smObjectType
            var operationTypes = protocol.GroupBy(g => g.Oper)
                .Select(u => new { Type = u.Key, Name = Convertor.GetInstance().GetOperationName(u.Key - 1) })
                .OrderBy(s => s.Name);

            var operationNodes = new ObservableCollection<Node>();

            foreach (var operationType in operationTypes)
            {
                // Records refered to current login, current smObjectType and current operationType
                var operationRecords = protocol.Where(o => o.Oper == operationType.Type);

                // Get all descriptions for current login, current smObjectType and current operationType
                var descriptionItems = operationRecords.Select(o => new {o.EvDate, ProjID = o.ProjId, SmObjID = o.SmObjId, o.LogText })
                    .OrderBy(p => p.EvDate);

                var descriptionNodes = new ObservableCollection<Node>();

                foreach (var descriptionItem in descriptionItems)
                {
                    var descriptionNode = new DescriptionNode(descriptionItem.EvDate, descriptionItem.ProjID,
                        descriptionItem.SmObjID, descriptionItem.LogText);

                    // Create path according which node is the first
                    switch (topNodeEnum)
                    {
                        case TopNodeEnum.Login:
                            descriptionNode.NodePath = $"{login}/{smObjectType}/{operationType.Name}";
                            break;
                        case TopNodeEnum.SmObjectType:
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
                switch (topNodeEnum)
                {
                    case TopNodeEnum.Login:
                        operationNode.NodePath = $"{login}/{smObjectType}";
                        break;
                    case TopNodeEnum.SmObjectType:
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
