import { useEffect, useState } from "react";

import { ListItemDefinition } from "../models";

const useListItemDefinition = (listId?: string) => {
  const [listItemDefinition, setListItemDefinition] =
    useState<ListItemDefinition | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const request = new Request(`/api/lists/${listId}/itemDefinition`, {
      method: "GET",
    });

    const fetchData = async () => {
      const response = await fetch(request);

      const data =
        response.status === 401 || response.status === 404
          ? null
          : await response.json();

      setListItemDefinition(data);
      setLoading(false);
    };

    fetchData();
  }, [listId]);

  return { listItemDefinition, loading };
};

export default useListItemDefinition;
