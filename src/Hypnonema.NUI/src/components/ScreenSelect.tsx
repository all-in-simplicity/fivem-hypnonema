import {FormControl, FormHelperText, InputLabel, MenuItem, Select, SelectChangeEvent} from "@mui/material";
import {FC} from "react";
import Screen from 'types/screen';

interface Props {
    screens: Screen[],
    onChange: Function
}

export const ScreenSelect: FC<Props> = (props) => {
    const handleChange = (event: SelectChangeEvent) => {
        props.onChange(event.target.value);
    }

    return (
        <FormControl fullWidth sx={{margin: 'auto', marginTop: '20px'}}>
            <InputLabel id="screen-select-label">Screen</InputLabel>
            <Select
                id="screen-select"
                defaultValue=''
                disabled={props.screens === undefined}
                onChange={handleChange}
                required
            >
                {props.screens.map((screen) => (
                    <MenuItem key={screen.id} value={screen.id}>{screen.name}</MenuItem>
                ))}
            </Select>
            {props.screens.length === 0 && <FormHelperText error>Create a Screen first</FormHelperText>}
        </FormControl>
    )
}
