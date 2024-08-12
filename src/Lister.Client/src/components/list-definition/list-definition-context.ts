import { GridColDef } from "@mui/x-data-grid";
import { createContext } from "react";

import { ListItemDefinition } from "../../models";

export interface ListItemDefinitionValue {
  getGridColDefs: (
    onItemClicked: (listId: string, itemId: string) => void
  ) => GridColDef[];
  listItemDefinition: ListItemDefinition | null;
}

const defaultValue: ListItemDefinitionValue = {
  getGridColDefs: () => [],
  listItemDefinition: null,
};

const ListItemDefinitionContext =
  createContext<ListItemDefinitionValue>(defaultValue);

export default ListItemDefinitionContext;
