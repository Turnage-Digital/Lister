import React, { useState } from "react";
import {
  Box,
  Button,
  FormControl,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import { Delete } from "@mui/icons-material";

import { ColumnDef, ColumnType } from "../../../api";

interface Props {
  propertyDefs: ColumnDef[] | null;
  updatePropertyDefs: (propertyDefs: ColumnDef[]) => void;
}

const ColumnsBlock = ({ propertyDefs, updatePropertyDefs }: Props) => {
  const [propertyName, setPropertyName] = useState<string | null>(null);
  const [propertyType, setPropertyType] = useState<ColumnType | null>(null);

  const handleAddClicked = () => {
    const notNull = propertyDefs ?? [];
    const added = [...notNull, { name: propertyName!, type: propertyType! }];

    setPropertyName(null);
    setPropertyType(null);
    updatePropertyDefs(added);
  };

  const handleRemoveClicked = (name: string) => {
    const notNull = propertyDefs ?? [];
    const updated = notNull.filter((pd) => pd.name !== name);

    updatePropertyDefs(updated);
  };

  const propertyTypeList = Object.values(ColumnType);

  return (
    <Stack spacing={2}>
      <Stack direction="row" spacing={2}>
        <TextField
          name="name"
          id="name"
          label="Name"
          margin="normal"
          fullWidth
          value={propertyName ?? ""}
          onChange={(event) => setPropertyName(event.target.value)}
          sx={{
            background: "white",
          }}
        />

        <FormControl variant="outlined" margin="normal" fullWidth>
          <InputLabel htmlFor="type">Type</InputLabel>
          <Select
            name="type"
            id="type"
            label="Type"
            value={propertyType ?? ""}
            onChange={(event) =>
              setPropertyType(event.target.value as ColumnType)
            }
            sx={{
              background: "white",
            }}
          >
            {propertyTypeList.map((propertyType) => (
              <MenuItem key={propertyType} value={propertyType}>
                {propertyType}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <Box sx={{ display: "flex", alignItems: "center" }}>
          <Button
            variant="contained"
            color="primary"
            onClick={handleAddClicked}
            disabled={!propertyName || !propertyType}
          >
            Add
          </Button>
        </Box>
      </Stack>

      {propertyDefs && propertyDefs.length > 0 && (
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Type</TableCell>
                <TableCell />
              </TableRow>
            </TableHead>
            <TableBody>
              {propertyDefs.map((propertyDef) => (
                <TableRow key={propertyDef.name}>
                  <TableCell>{propertyDef.name}</TableCell>
                  <TableCell>{propertyDef.type}</TableCell>
                  <TableCell>
                    <IconButton
                      onClick={() => handleRemoveClicked(propertyDef.name)}
                    >
                      <Delete />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Stack>
  );
};

export default ColumnsBlock;
