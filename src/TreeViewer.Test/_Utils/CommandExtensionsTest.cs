using Reactive.Bindings;

namespace TreeViewer
{
    public class CommandExtensionsTest
    {
        #region Static Methods

        [Fact]
        public async Task ToDelegate_WithAsyncReactiveCommandNonGeneric()
        {
            bool done = false;
            using AsyncReactiveCommand command = new AsyncReactiveCommand().WithSubscribe(async Task () =>
            {
                done = true;
                await Task.CompletedTask;
            });

            Func<Task> func = command.ToDelegate();
            Assert.False(done);

            await func.Invoke();
            Assert.True(done);
        }

        [Fact]
        public async Task ToDelegate_WithAsyncReactiveCommandAsGeneric()
        {
            bool done = false;
            using AsyncReactiveCommand<string> command = new AsyncReactiveCommand<string>().WithSubscribe(async Task (x) =>
            {
                done = true;
                await Task.CompletedTask;
            });

            Func<string, Task> func = command.ToDelegate();
            Assert.False(done);

            await func.Invoke("hoge");
            Assert.True(done);
        }

        [Fact]
        public async Task ToDelegate_WithAsyncReactiveCommandAsGenericAndArgument()
        {
            string? arg = null;
            using AsyncReactiveCommand<string> command = new AsyncReactiveCommand<string>().WithSubscribe(async Task (x) =>
            {
                arg = x;
                await Task.CompletedTask;
            });

            Func<Task> func = command.ToDelegate("hoge");
            Assert.Null(arg);

            await func.Invoke();
            Assert.Equal("hoge", arg);
        }

        #endregion Static Methods
    }
}
