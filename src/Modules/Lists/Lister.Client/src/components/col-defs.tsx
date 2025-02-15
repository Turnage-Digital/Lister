import * as React from "react";

import {Delete, Edit, Visibility} from "@mui/icons-material";
import {GridActionsCellItem, GridColDef} from "@mui/x-data-grid";

import {Column, getStatusFromName, ListItemDefinition} from "../models";
import StatusChip from "./status-chip";

export const getGridColDefs = (
    listItemDefinition: ListItemDefinition,
    handleViewClicked: (listId: string, itemId: number) => void,
    handleDeleteClicked: (listId: string, itemId: number) => void,
): GridColDef[] => {
    const retval: GridColDef[] = [];

    retval.push({
        field: "id",
        headerName: "ID",
        width: 75,
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
        width: 75,
        cellClassName: "actions",
        getActions: ({id}) => {
            return [
                <GridActionsCellItem
                    key={`${id}-view`}
                    showInMenu
                    icon={<Visibility/>}
                    label="View"
                    color="primary"
                    onClick={() =>
                        handleViewClicked(listItemDefinition.id!, id as number)
                    }
                />,
                <GridActionsCellItem
                    key={`${id}-edit`}
                    showInMenu
                    icon={<Edit/>}
                    label="Edit"
                    color="primary"
                />,
                <GridActionsCellItem
                    key={`${id}-delete`}
                    showInMenu
                    icon={<Delete/>}
                    label="Delete"
                    color="primary"
                    onClick={() =>
                        handleDeleteClicked(listItemDefinition.id!, id as number)
                    }
                />,
            ];
        },
    });

    return retval;
};
