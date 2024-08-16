// import React from "react";
// import { Paper, Stack } from "@mui/material";
// import { AddCircle, MoreVert, Visibility } from "@mui/icons-material";
// import {
//   DataGrid,
//   GridActionsCellItem,
//   GridColDef,
//   GridPaginationModel,
//   GridSortModel,
// } from "@mui/x-data-grid";
// import { useQuery } from "@tanstack/react-query";
// import { useNavigate, useParams, useSearchParams } from "react-router-dom";
//
// import {
//   Column,
//   getStatusFromName,
//   Item,
//   type ListItemDefinition,
// } from "../../models";
// import { StatusChip, Titlebar } from "../../components";
//
// const ListPage = () => {
//   const navigate = useNavigate();
//   const { listId } = useParams();
//   const [searchParams, setSearchParams] = useSearchParams();
//
//   const listDefinitionQuery = useQuery<ListItemDefinition>({
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
//   const pagedItemsQuery = useQuery<{ items: Item[]; count: number }>({
//     queryKey: ["list-items", listId, searchParams.toString()],
//     queryFn: async () => {
//       const page = Number(searchParams.get("page") ?? "0");
//       const pageSize = Number(searchParams.get("pageSize") ?? "10");
//       const field = searchParams.get("field");
//       const sort = searchParams.get("sort");
//
//       let url = `/api/lists/${listId}/items?page=${page}&pageSize=${pageSize}`;
//       if (field && sort) {
//         url += `&field=${field}&sort=${sort}`;
//       }
//       const request = new Request(url, {
//         method: "GET",
//       });
//       const response = await fetch(request);
//       const retval = await response.json();
//       return retval;
//     },
//     enabled: !!listId,
//   });
//
//   const handlePaginationChange = (gridPaginationModel: GridPaginationModel) => {
//     const page = gridPaginationModel.page;
//     const pageSize = gridPaginationModel.pageSize;
//
//     searchParams.set("page", page.toString());
//     searchParams.set("pageSize", pageSize.toString());
//
//     setSearchParams(searchParams);
//   };
//
//   const handleSortChange = (gridSortModel: GridSortModel) => {
//     if (gridSortModel.length === 0) {
//       searchParams.delete("field");
//       searchParams.delete("sort");
//     } else {
//       const field = gridSortModel[0].field;
//       const sort = gridSortModel[0].sort === "desc" ? "desc" : "asc";
//
//       searchParams.set("field", field);
//       searchParams.set("sort", sort);
//     }
//
//     setSearchParams(searchParams);
//   };
//
//   const handleItemClicked = (listId: string, itemId: string) => {
//     navigate(`/${listId}/items/${itemId}`);
//   };
//
//   if (!listDefinitionQuery.isSuccess || !pagedItemsQuery.isSuccess) {
//     return null;
//   }
//
//   const getPaginationFromSearchParams = (
//     searchParams: URLSearchParams
//   ): GridPaginationModel => {
//     const page = Number(searchParams.get("page") ?? "0");
//     const pageSize = Number(searchParams.get("pageSize") ?? "10");
//
//     return { page, pageSize };
//   };
//
//   const getSortFromSearchParams = (
//     searchParams: URLSearchParams
//   ): GridSortModel => {
//     if (!searchParams.has("field")) {
//       return [];
//     }
//
//     const field = searchParams.get("field")!;
//     const sort = searchParams.get("sort") === "desc" ? "desc" : "asc";
//
//     return [{ field, sort }];
//   };
//
//   const getGridColDefs = (
//     onItemClicked: (listId: string, itemId: string) => void
//   ): GridColDef[] => {
//     const retval: GridColDef[] = [];
//
//     retval.push({
//       field: "id",
//       headerName: "ID",
//       width: 100,
//       sortable: false,
//       disableColumnMenu: true,
//     });
//
//     const mapped = listDefinitionQuery.data.columns.map((column: Column) => {
//       const retval: GridColDef = {
//         field: column.property!,
//         headerName: column.name,
//         flex: 1,
//       };
//
//       if (column.type === "Date") {
//         retval.valueFormatter = (params) => {
//           const date = new Date(params.value);
//           const retval = date.toLocaleDateString();
//           return retval;
//         };
//       }
//       return retval;
//     });
//
//     retval.push(...mapped);
//
//     retval.push({
//       field: "status",
//       headerName: "Status",
//       width: 150,
//       renderCell: (params) => (
//         <StatusChip
//           status={getStatusFromName(
//             listDefinitionQuery.data.statuses,
//             params.value
//           )}
//         />
//       ),
//     });
//
//     retval.push({
//       field: "actions",
//       type: "actions",
//       headerName: "",
//       width: 100,
//       cellClassName: "actions",
//       getActions: ({ id }) => {
//         return [
//           <GridActionsCellItem
//             key={`${id}-view`}
//             icon={<Visibility />}
//             label="View"
//             color="primary"
//             onClick={() => onItemClicked(listId!, id as string)}
//           />,
//           <GridActionsCellItem
//             key={`${id}-delete`}
//             icon={<MoreVert />}
//             label="More"
//             color="primary"
//           />,
//         ];
//       },
//     });
//
//     return retval;
//   };
//
//   const gridColDefs = getGridColDefs(handleItemClicked);
//   const pagination = getPaginationFromSearchParams(searchParams);
//   const sort = getSortFromSearchParams(searchParams);
//
//   const rows = pagedItemsQuery.data.items.map((item: Item) => ({
//     id: item.id,
//     ...item.bag,
//   }));
//
//   const actions = [
//     {
//       title: "Add an Item",
//       icon: <AddCircle />,
//       onClick: () => navigate(`/${listId}/items/create`),
//     },
//   ];
//
//   const breadcrumbs = [
//     {
//       title: "Lists",
//       onClick: () => navigate("/"),
//     },
//   ];
//
//   return (
//     <Stack sx={{ px: 2, py: 4 }}>
//       <Titlebar
//         title={listDefinitionQuery.data.name}
//         actions={actions}
//         breadcrumbs={breadcrumbs}
//       />
//
//       <Paper sx={{ my: 4 }}>
//         <DataGrid
//           columns={gridColDefs}
//           rows={rows}
//           getRowId={(row) => row.id}
//           rowCount={pagedItemsQuery.data.count}
//           paginationMode="server"
//           paginationModel={pagination}
//           pageSizeOptions={[10, 25, 50]}
//           onPaginationModelChange={handlePaginationChange}
//           sortingMode="server"
//           sortModel={sort}
//           onSortModelChange={handleSortChange}
//           disableColumnFilter
//           disableColumnSelector
//           disableRowSelectionOnClick
//         />
//       </Paper>
//     </Stack>
//   );
// };
//
// export default ListPage;
