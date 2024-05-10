import React, { PropsWithChildren, useEffect, useMemo, useState } from "react";

import { ListItemDefinition, IListsApi, ListsApi } from "../../api";
import Loading from "../loading";

import ListDefinitionContext from "./list-definition-context";

type Props = PropsWithChildren;

const listApi: IListsApi = new ListsApi(`/api/lists`);

const ListDefinitionProvider = ({ children }: Props) => {
  const [listId, setListId] = useState<string | null>(null);
  const [listItemDefinition, setListItemDefinition] =
    useState<ListItemDefinition | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!listId) {
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
  }, [listId]);

  const listDefinitionContextValue = useMemo(
    () => ({ setListId, listItemDefinition, error }),
    [setListId, listItemDefinition, error]
  );

  const content = loading ? <Loading /> : children;
  return (
    <ListDefinitionContext.Provider value={listDefinitionContextValue}>
      {content}
    </ListDefinitionContext.Provider>
  );
};

export default ListDefinitionProvider;
