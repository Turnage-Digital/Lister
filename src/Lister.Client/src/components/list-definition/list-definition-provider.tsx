// import React, { PropsWithChildren, useMemo } from "react";
// import { MoreVert, Visibility } from "@mui/icons-material";
// import { GridActionsCellItem, GridColDef } from "@mui/x-data-grid";
// import { useQuery } from "@tanstack/react-query";
// import { useParams } from "@tanstack/react-router";
//
// import { Column, getStatusFromName } from "../../models";
// import StatusChip from "../status-chip";
//
// import ListDefinitionContext from "./list-definition-context";
//
// const ListDefinitionProvider = ({ children }: PropsWithChildren) => {
//   const { listId } = useParams({ from: "/api/lists/${listId}" });
//   const query = useQuery({
//     queryKey: ["list-definition", listId],
//     queryFn: async () => {
//       const request = new Request(`/api/lists/${listId}/itemDefinition`, {
//         method: "GET",
//       });
//       const response = await fetch(request);
//       const retval = await response.json();
//       return retval;
//     },
//     enabled: !!listId,
//   });
//
//   const listDefinitionContextValue = useMemo(() => {
//     const getGridColDefs = (
//       onItemClicked: (listId: string, itemId: string) => void
//     ): GridColDef[] => {
//       if (!query.isSuccess || !query.data) {
//         return [];
//       }
//
//       const retval: GridColDef[] = [];
//
//       retval.push({
//         field: "id",
//         headerName: "ID",
//         width: 100,
//         sortable: false,
//         disableColumnMenu: true,
//       });
//
//       const mapped = query.data.columns.map((column: Column) => {
//         const retval: GridColDef = {
//           field: column.property!,
//           headerName: column.name,
//           flex: 1,
//         };
//
//         if (column.type === "Date") {
//           retval.valueFormatter = (params) => {
//             const date = new Date(params.value);
//             const retval = date.toLocaleDateString();
//             return retval;
//           };
//         }
//         return retval;
//       });
//
//       retval.push(...mapped);
//
//       retval.push({
//         field: "status",
//         headerName: "Status",
//         width: 150,
//         renderCell: (params) => (
//           <StatusChip
//             status={getStatusFromName(query.data.statuses, params.value)}
//           />
//         ),
//       });
//
//       retval.push({
//         field: "actions",
//         type: "actions",
//         headerName: "",
//         width: 100,
//         cellClassName: "actions",
//         getActions: ({ id }) => {
//           return [
//             <GridActionsCellItem
//               key={`${id}-view`}
//               icon={<Visibility />}
//               label="View"
//               color="primary"
//               onClick={() => onItemClicked(listId!, id as string)}
//             />,
//             <GridActionsCellItem
//               key={`${id}-delete`}
//               icon={<MoreVert />}
//               label="More"
//               color="primary"
//             />,
//           ];
//         },
//       });
//
//       return retval;
//     };
//
//     return { listItemDefinition: query.data, getGridColDefs };
//   }, [listId, query]);
//
//   return (
//     <ListDefinitionContext.Provider value={listDefinitionContextValue}>
//       {children}
//     </ListDefinitionContext.Provider>
//   );
// };
//
// export default ListDefinitionProvider;
