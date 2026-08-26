List<Agent> agents = new List<Agent>();

void Setup()
{
    Screen.Size(640, 360);
    agents.Add(new Agent(80, 80, Behaviour.Seek, Colour.Yellow));
    agents.Add(new Agent(560, 80, Behaviour.Seek, Colour.Orange));
    agents.Add(new Agent(80, 280, Behaviour.Flee, Colour.Pink));
    agents.Add(new Agent(560, 280, Behaviour.Flee, Colour.Purple));
    agents.Add(new Agent(320, 180, Behaviour.Wander, Colour.Cyan));
}

void Draw()
{
    Screen.Clear(Colour.Black);
    Vec2 pointer = new Vec2(Mouse.X, Mouse.Y);
    foreach (Agent agent in agents)
    {
        agent.Update(pointer);
        agent.Draw();
    }
    Screen.Circle(pointer.x, pointer.y, 12, Colour.Grey);
}

Game.Run(Setup, Draw);

class Agent
{
    public Vec2 position;
    public Vec2 velocity = new Vec2(1, 1);
    public Behaviour behaviour;
    public Colour colour;
    public float maxSpeed = 4;
    public float steerForce = 0.4f;
    public float radius = 14;

    public Agent(float startX, float startY, Behaviour how, Colour tint)
    {
        position = new Vec2(startX, startY);
        behaviour = how;
        colour = tint;
    }

    public void Update(Vec2 pointer)
    {
        // Where this agent would like to head, which is the only line the three behaviours differ on.
        Vec2 push = new Vec2(0, 0);
        switch (behaviour)
        {
            case Behaviour.Seek: push = new Vec2(pointer.x - position.x, pointer.y - position.y); break;
            case Behaviour.Flee: push = new Vec2(position.x - pointer.x, position.y - pointer.y); break;
            case Behaviour.Wander: push = new Vec2(Rand.Range(-0.5f, 0.5f), Rand.Range(-0.5f, 0.5f)); break;
        }
        // An arrow of length 0 has no direction, and normalising it would divide by zero.
        if (push.Length() > 0.01f) velocity = velocity.Add(push.Normalised().Scale(steerForce));
        // Back to one steady speed every frame: nobody stops, and nobody runs away.
        velocity = velocity.Normalised().Scale(maxSpeed);
        position = position.Add(velocity);
        // Off one edge and on at the other, so an agent that overshoots always comes back.
        if (position.x < -radius) position.x = Screen.Width + radius;
        if (position.x > Screen.Width + radius) position.x = -radius;
        if (position.y < -radius) position.y = Screen.Height + radius;
        if (position.y > Screen.Height + radius) position.y = -radius;
    }

    public void Draw()
    {
        Screen.Circle(position.x, position.y, radius, colour);
    }
}

enum Behaviour { Seek, Flee, Wander }

class Vec2
{
    public float x, y;

    public Vec2(float startX, float startY)
    {
        x = startX;
        y = startY;
    }

    public float Length()
    {
        return MathF.Sqrt(x * x + y * y);
    }

    public Vec2 Normalised()
    {
        float length = Length();
        return new Vec2(x / length, y / length);
    }

    public Vec2 Add(Vec2 other)
    {
        return new Vec2(x + other.x, y + other.y);
    }

    public Vec2 Scale(float amount)
    {
        return new Vec2(x * amount, y * amount);
    }
}
