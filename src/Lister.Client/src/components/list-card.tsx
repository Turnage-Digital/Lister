import { Delete, Visibility } from "@mui/icons-material";
import {
  Card,
  CardActions,
  CardContent,
  Grid2,
  IconButton,
  Typography,
} from "@mui/material";
import React from "react";

import { ListName } from "../models";

export interface Props {
  listName: ListName;
  onViewClick: (id: string) => void;
}

const ListCard = ({ listName, onViewClick }: Props) => {
  return (
    <Grid2 key={listName.id} size={{ xs: 12, sm: 6, md: 4 }}>
      <Card>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {listName.name}
          </Typography>
        </CardContent>
        <CardActions sx={{ justifyContent: "flex-end" }}>
          <IconButton onClick={() => onViewClick(listName.id)}>
            <Visibility />
          </IconButton>
          <IconButton>
            <Delete />
          </IconButton>
        </CardActions>
      </Card>
    </Grid2>
  );
};

export default ListCard;
