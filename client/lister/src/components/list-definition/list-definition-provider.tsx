import { MoreVert, Visibility } from "@mui/icons-material";
import { GridActionsCellItem, GridColDef } from "@mui/x-data-grid";
import React, { PropsWithChildren, useEffect, useMemo, useState } from "react";
import { useParams } from "react-router-dom";

import {
  Column,
  getStatusFromName,
  IListsApi,
  ListItemDefinition,
  ListsApi,
} from "../../api";
import { useAuth } from "../auth";
import { useLoad } from "../load";
import StatusChip from "../status-chip";

import ListDefinitionContext from "./list-definition-context";

type Props = PropsWithChildren;

const listApi: IListsApi = new ListsApi(`/api/lists`);

const ListDefinitionProvider = ({ children }: Props) => {
  const { setLoading } = useLoad();
  const { loggedIn } = useAuth();

  const { listId } = useParams();

  const [listItemDefinition, setListItemDefinition] =
    useState<ListItemDefinition | null>(null);

  useEffect(() => {
    if (!listId || !loggedIn) {
      setListItemDefinition(null);
      return;
    }

    const fetchData = async () => {
      setLoading(true);
      try {
        const result = await listApi.getListItemDefinition(listId);
        setListItemDefinition(result);
      } catch (e: any) {
        // setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [listId, loggedIn, setLoading]);

  const listDefinitionContextValue = useMemo(() => {
    const getGridColDefs = (
      onItemClicked: (listId: string, itemId: string) => void
    ): GridColDef[] => {
      if (!listItemDefinition) {
        return [];
      }

      const retval: GridColDef[] = [];

      retval.push({
        field: "id",
        headerName: "Id",
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
            const date = new Date(params.value);
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
            status={getStatusFromName(
              listItemDefinition.statuses,
              params.value
            )}
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
              onClick={() => onItemClicked(listId!, id as string)}
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

    return { getGridColDefs, listItemDefinition };
  }, [listId, listItemDefinition]);

  return (
    <ListDefinitionContext.Provider value={listDefinitionContextValue}>
      {children}
    </ListDefinitionContext.Provider>
  );
};

export default ListDefinitionProvider;
