import { createContext } from "react";

import { ListItemDefinition } from "../../api";

export interface ListItemDefinitionContextProps {
  setListId: (listId: string) => void;
  listItemDefinition: ListItemDefinition | null;
  error: string | null;
}

const defaultValue: ListItemDefinitionContextProps = {
  setListId: () => {},
  listItemDefinition: null,
  error: null,
};

const ListItemDefinitionContext =
  createContext<ListItemDefinitionContextProps>(defaultValue);

export default ListItemDefinitionContext;
