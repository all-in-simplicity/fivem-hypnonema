import { FC } from "react";
import Schedule from "../../types/schedule";
import {
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import Paper from "@mui/material/Paper";
import { getNextScheduleOccurrence, getScheduleRuleText } from "../../utils";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import { format, parseISO } from "date-fns";
import { enGB } from "date-fns/locale";

interface SchedulesTableProps {
  schedules: Schedule[] | undefined;
  onDelete: Function;
  onEdit: Function;
}

export const SchedulesTable: FC<SchedulesTableProps> = (props) => {
  return (
    <TableContainer component={Paper}>
      <Table sx={{ minWidth: 530 }} size="small">
        <TableHead>
          <TableRow>
            <TableCell></TableCell>
            <TableCell align="left">Screen</TableCell>
            <TableCell align="left">Url</TableCell>
            <TableCell align="left">Recurrence</TableCell>
            <TableCell align="left">Next</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {props.schedules?.length !== 0 ? (
            props.schedules?.map((schedule) => (
              <TableRow
                key={schedule.id}
                sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
              >
                <TableCell>
                  <IconButton
                    aria-label="delete"
                    onClick={() => props.onDelete(schedule)}
                    size="small"
                  >
                    <DeleteIcon fontSize="inherit" />
                  </IconButton>
                  <IconButton
                    aria-label="edit"
                    onClick={() => props.onEdit(schedule)}
                    size="small"
                  >
                    <EditIcon fontSize="inherit" />
                  </IconButton>
                </TableCell>
                <TableCell component="th" scope="row">
                  {schedule.screen?.name}
                </TableCell>
                <TableCell>{schedule.url}</TableCell>
                <TableCell>
                  {getScheduleRuleText(schedule)} at{" "}
                  {format(parseISO(schedule.startDateTime), "p", {
                    locale: enGB,
                  })}{" "}
                </TableCell>

                <TableCell>{getNextScheduleOccurrence(schedule)}</TableCell>
              </TableRow>
            ))
          ) : (
            <TableRow
              sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
            >
              <TableCell align="center" colSpan={2}>
                No schedules existing.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
};
