import { MoreVert, Visibility } from "@mui/icons-material";
import { GridActionsCellItem, GridColDef } from "@mui/x-data-grid";
import React from "react";

import { Column, getStatusFromName, ListItemDefinition } from "../models";

import StatusChip from "./status-chip";

export const getGridColDefs = (
  listItemDefinition: ListItemDefinition,
  handleItemClicked: (listId: string, itemId: string) => void,
): GridColDef[] => {
  const retval: GridColDef[] = [];

  retval.push({
    field: "id",
    headerName: "ID",
    width: 100,
    sortable: false,
    disableColumnMenu: true,
  });

  const mapped = listItemDefinition.columns.map((column: Column) => {
    const retval: GridColDef = {
      field: column.property!,
      headerName: column.name,
      flex: 1,
    };

    if (column.type === "Date") {
      retval.valueFormatter = (params) => {
        const date = new Date(params);
        const retval = date.toLocaleDateString();
        return retval;
      };
    }
    return retval;
  });

  retval.push(...mapped);

  retval.push({
    field: "status",
    headerName: "Status",
    width: 150,
    renderCell: (params) => (
      <StatusChip
        status={getStatusFromName(listItemDefinition.statuses, params.value)}
      />
    ),
  });

  retval.push({
    field: "actions",
    type: "actions",
    headerName: "",
    width: 100,
    cellClassName: "actions",
    getActions: ({ id }) => {
      return [
        <GridActionsCellItem
          key={`${id}-view`}
          icon={<Visibility />}
          label="View"
          color="primary"
          onClick={() =>
            handleItemClicked(listItemDefinition.id!, id.toString())
          }
        />,
        <GridActionsCellItem
          key={`${id}-delete`}
          icon={<MoreVert />}
          label="More"
          color="primary"
        />,
      ];
    },
  });

  return retval;
};
