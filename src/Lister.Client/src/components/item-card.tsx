import React from "react";
import { Card, CardHeader } from "@mui/material";

import { Item } from "../models";

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
