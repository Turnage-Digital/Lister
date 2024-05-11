import { GridColDef } from "@mui/x-data-grid";
import { createContext } from "react";

import { ListItemDefinition } from "../../api";

export interface ListItemDefinitionContextProps {
  setListId: (listId: string) => void;
  getGridColDefs: () => GridColDef[];
  listItemDefinition: ListItemDefinition | null;
  error: string | null;
}

const defaultValue: ListItemDefinitionContextProps = {
  setListId: () => {},
  getGridColDefs: () => [],
  listItemDefinition: null,
  error: null,
};

const ListItemDefinitionContext =
  createContext<ListItemDefinitionContextProps>(defaultValue);

export default ListItemDefinitionContext;
