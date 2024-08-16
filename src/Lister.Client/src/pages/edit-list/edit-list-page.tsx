// import React, { FormEvent, useEffect, useState } from "react";
// import { Box, Button, Divider, Stack } from "@mui/material";
// import { Save } from "@mui/icons-material";
// import { useNavigate, useSubmit } from "react-router-dom";
//
// import { ListItemDefinition } from "../../models";
// import { FormBlock, Titlebar } from "../../components";
//
// import ColumnsContent from "./columns-content";
// import NameBlock from "./name-block";
// import StatusesContent from "./statuses-content";
//
// const EditListPage = () => {
//   const navigate = useNavigate();
//   const submit = useSubmit();
//
//   const defaultListDefinition: ListItemDefinition = {
//     id: null,
//     name: "",
//     columns: [],
//     statuses: [],
//   };
//
//   const [updated, setUpdated] = useState<ListItemDefinition>(() => {
//     const item = window.sessionStorage.getItem("updated_list");
//     return item ? JSON.parse(item) : defaultListDefinition;
//   });
//
//   useEffect(() => {
//     window.sessionStorage.setItem("updated_list", JSON.stringify(updated));
//   }, [updated]);
//
//   const update = (key: keyof ListItemDefinition, value: any) => {
//     setUpdated((prev) => ({ ...prev, [key]: value }));
//   };
//
//   const handleSubmit = async (e: FormEvent) => {
//     e.preventDefault();
//
//     const data = {
//       serialized: JSON.stringify(updated),
//     };
//
//     submit(data, {
//       method: "post",
//     });
//
//     window.sessionStorage.removeItem("updated_list");
//   };
//
//   const breadcrumbs = [
//     {
//       title: "Lists",
//       onClick: () => navigate("/"),
//     },
//   ];
//
//   return (
//     <Stack
//       component="form"
//       spacing={4}
//       divider={<Divider />}
//       onSubmit={handleSubmit}
//       sx={{ px: 2, py: 4 }}
//     >
//       <Titlebar title="Create a List" breadcrumbs={breadcrumbs} />
//
//       <FormBlock
//         title="Name"
//         blurb="Blurb about naming a list."
//         content={
//           <NameBlock
//             name={updated.name}
//             onNameChanged={(name) => update("name", name)}
//           />
//         }
//       />
//
//       <FormBlock
//         title="Columns"
//         blurb="Blurb about columns for a list."
//         content={
//           <ColumnsContent
//             columns={updated.columns}
//             onColumnsChanged={(columns) => update("columns", columns)}
//           />
//         }
//       />
//
//       <FormBlock
//         title="Statuses"
//         blurb="Blurb about statuses for an item."
//         content={
//           <StatusesContent
//             statuses={updated.statuses}
//             onStatusesChanged={(statuses) => update("statuses", statuses)}
//           />
//         }
//       />
//
//       <Box
//         sx={{
//           display: "flex",
//           justifyContent: { xs: "center", md: "flex-end" },
//         }}
//       >
//         <Button
//           type="submit"
//           variant="contained"
//           startIcon={<Save />}
//           sx={{ width: { xs: "100%", md: "auto" } }}
//         >
//           Submit
//         </Button>
//       </Box>
//     </Stack>
//   );
// };
//
// export default EditListPage;
