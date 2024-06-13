import { Grid, Typography } from "@mui/material";

function App() {
  return (
    <Grid container spacing={2}>
      <Grid item md={2}></Grid>
      <Grid item md={8}>
        <Typography gutterBottom variant="h2">
          Hello World
        </Typography>
      </Grid>
      <Grid item md={2}></Grid>
    </Grid>
  );
}

export default App;
