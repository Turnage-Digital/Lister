import { GridColDef } from "@mui/x-data-grid";
import { createContext } from "react";

import { ListItemDefinition } from "../../api";

export interface ListItemDefinitionContextProps {
  setListId: (listId: string) => void;
  getGridColDefs: (
    onItemClicked: (listId: string, itemId: string) => void
  ) => GridColDef[];
  listItemDefinition: ListItemDefinition | null;
}

const defaultValue: ListItemDefinitionContextProps = {
  setListId: () => {},
  getGridColDefs: () => [],
  listItemDefinition: null,
};

const ListItemDefinitionContext =
  createContext<ListItemDefinitionContextProps>(defaultValue);

export default ListItemDefinitionContext;
