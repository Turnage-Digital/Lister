import { Card, CardHeader } from "@mui/material";
import React from "react";

import { Item } from "../api";

export interface Props {
  item: Item;
}

const ItemCard = ({ item }: Props) => {
  return (
    <Card>
      <CardHeader title={`Item # ${item.id}`} />
    </Card>
  );
};

export default ItemCard;
