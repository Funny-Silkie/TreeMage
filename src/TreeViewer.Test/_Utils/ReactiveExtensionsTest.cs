﻿using Reactive.Bindings;

namespace TreeViewer
{
    public class ReactiveExtensionsTest
    {
        #region Static Methods

        [Fact]
        public void WithSubscribe_WithReactivePropertySlim()
        {
            string? arg = null;
            using var property = new ReactivePropertySlim<string?>();

            Assert.Same(property, property.WithSubscribe(x => arg = x));
            Assert.Null(arg);

            property.Value = "hoge";
            Assert.Equal("hoge", arg);
        }

        [Fact]
        public async Task WithSubscribe_WithAsyncReactiveCommand_NonGeneric()
        {
            bool done = false;
            using var command = new AsyncReactiveCommand();

            Assert.Same(command, command.WithSubscribe(() => done = true));
            Assert.False(done);

            await command.ExecuteAsync();
            Assert.True(done);
        }

        [Fact]
        public async Task WithSubscribe_WithAsyncReactiveCommand_Generic()
        {
            string? arg = null;
            using var command = new AsyncReactiveCommand<string>();

            Assert.Same(command, command.WithSubscribe(x => arg = x));
            Assert.Null(arg);

            await command.ExecuteAsync("hoge");
            Assert.Equal("hoge", arg);
        }

        #endregion Static Methods
    }
}
