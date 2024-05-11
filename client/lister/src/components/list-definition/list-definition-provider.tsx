import { MoreVert, Visibility } from "@mui/icons-material";
import { GridActionsCellItem, GridColDef } from "@mui/x-data-grid";
import React, { PropsWithChildren, useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { Column, IListsApi, ListItemDefinition, ListsApi } from "../../api";
import { getStatusFromName } from "../../status-fns";
import { useAuth } from "../auth";
import Loading from "../loading";
import StatusChip from "../status-chip";

import ListDefinitionContext from "./list-definition-context";

type Props = PropsWithChildren;

const listApi: IListsApi = new ListsApi(`/api/lists`);

const ListDefinitionProvider = ({ children }: Props) => {
  const { signedIn } = useAuth();

  const [listId, setListId] = useState<string | null>(null);
  const [listItemDefinition, setListItemDefinition] =
    useState<ListItemDefinition | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!listId || !signedIn) {
      return;
    }

    const fetchData = async () => {
      setLoading(true);

      try {
        const result = await listApi.getListItemDefinition(listId);
        setListItemDefinition(result);
      } catch (e: any) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [listId, signedIn]);

  const listDefinitionContextValue = useMemo(() => {
    const getGridColDefs = (): GridColDef[] => {
      if (!listItemDefinition) {
        return [];
      }

      const retval: GridColDef[] = [];

      retval.push({
        field: "id",
        headerName: "Item #",
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
              // onClick={() => navigate(`/${params.listId}/items/${id}`)}
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

    return { setListId, getGridColDefs, listItemDefinition, error };
  }, [setListId, listItemDefinition, error]);

  const content = loading ? <Loading /> : children;
  return (
    <ListDefinitionContext.Provider value={listDefinitionContextValue}>
      {children}
    </ListDefinitionContext.Provider>
  );
};

export default ListDefinitionProvider;
